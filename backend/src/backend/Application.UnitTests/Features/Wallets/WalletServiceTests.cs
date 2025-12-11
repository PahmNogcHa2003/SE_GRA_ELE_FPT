using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Services.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Persistence;
using Infrastructure.Repositories.User;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Services
{
    public class WalletServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IUnitOfWork _uow;
        private readonly Mock<IWalletRepository> _walletRepo;
        private readonly Mock<IWalletDebtRepository> _walletDebtRepo;
        private readonly Mock<IWalletTransactionRepository> _txnRepo;
        private readonly IMapper _mapper;

        private readonly WalletService _service;

        public WalletServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);
            _uow = new UnitOfWork(_context);

            _walletRepo = new Mock<IWalletRepository>();
            _walletDebtRepo = new Mock<IWalletDebtRepository>();
            _txnRepo = new Mock<IWalletTransactionRepository>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Wallet, WalletDTO>();
            });
            _mapper = mapperConfig.CreateMapper();

            var baseRepo = new Mock<IRepository<Wallet, long>>();

            _service = new WalletService(
                baseRepo.Object,
                _mapper,
                _uow,
                _walletDebtRepo.Object,
                _walletRepo.Object,
                _txnRepo.Object
            );
        }

        [Fact]
        public async Task CreditAsync_ShouldCreateWalletIfNotExist()
        {
            long userId = 1;
            decimal amount = 100;

            _walletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Wallet?)null);

            var walletInDb = new Wallet { Id = 10, UserId = userId, Balance = 0 };
            _walletRepo.Setup(r => r.AddAsync(It.IsAny<Wallet>(), It.IsAny<CancellationToken>()))
                       .Callback<Wallet, CancellationToken>((w, ct) =>
                       {
                           walletInDb = w;
                           w.Id = 10;
                           _context.Wallets.Add(w);
                           _context.SaveChanges();
                       })
                       .Returns(Task.CompletedTask);

            _walletDebtRepo.Setup(r => r.GetUnpaidDebtsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new List<WalletDebt>());

            _txnRepo.Setup(r => r.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var txn = await _service.CreditAsync(userId, amount, "TopUp", null, CancellationToken.None);

            txn.Amount.Should().Be(100);
            walletInDb.Balance.Should().Be(100);
        }

        [Fact]
        public async Task CreditAsync_ShouldPayDebtsFirst()
        {
            long userId = 2;
            decimal amount = 100;

            var wallet = new Wallet { Id = 5, UserId = userId, Balance = 0, TotalDebt = 50 };
            _walletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(wallet);

            var debts = new List<WalletDebt>
            {
                new WalletDebt { Id = 1, UserId = userId, OrderId = 999, Remaining = 50, Status = WalletDebtStatus.Unpaid }
            };

            _walletDebtRepo.Setup(r => r.GetUnpaidDebtsByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(debts);

            _txnRepo.Setup(r => r.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var txn = await _service.CreditAsync(userId, amount, "TopUp", null, CancellationToken.None);

            debts[0].Remaining.Should().Be(0);
            wallet.TotalDebt.Should().Be(0);
            wallet.Balance.Should().Be(50);
            txn.Amount.Should().Be(50);
        }

        [Fact]
        public async Task CreditPromoAsync_ShouldIncreasePromoBalance()
        {
            long userId = 3;

            var wallet = new Wallet
            {
                Id = 3,
                UserId = userId,
                Balance = 0,
                PromoBalance = 10
            };

            _walletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(wallet);

            _walletRepo.Setup(r => r.Update(It.IsAny<Wallet>()));

            _txnRepo.Setup(r => r.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var txn = await _service.CreditPromoAsync(userId, 20, "PromoAdd", CancellationToken.None);

            wallet.PromoBalance.Should().Be(30);
            txn.PromoAfter.Should().Be(30);
        }

        [Fact]
        public async Task ConvertPromoToBalanceAsync_ShouldConvertSuccessfully()
        {
            long userId = 4;

            var wallet = new Wallet
            {
                Id = 4,
                UserId = userId,
                Balance = 0,
                PromoBalance = 50,
                Status = "Active"
            };

            _walletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(wallet);

            _txnRepo.Setup(r => r.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var txn = await _service.ConvertPromoToBalanceAsync(userId, 30, CancellationToken.None);

            wallet.PromoBalance.Should().Be(20);
            wallet.Balance.Should().Be(30);
            txn.Amount.Should().Be(30);
        }

        [Fact]
        public async Task ConvertPromoToBalanceAsync_ShouldFail_WhenPromoNotEnough()
        {
            long userId = 5;

            var wallet = new Wallet
            {
                Id = 5,
                UserId = userId,
                Balance = 0,
                PromoBalance = 10,
                Status = "Active"
            };

            _walletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(wallet);

            Func<Task> act = async () =>
                await _service.ConvertPromoToBalanceAsync(userId, 20, CancellationToken.None);

            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Promo balance is not enough to convert.");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
