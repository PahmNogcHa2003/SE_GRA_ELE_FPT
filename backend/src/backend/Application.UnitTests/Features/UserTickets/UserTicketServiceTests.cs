using Application.DTOs.Tickets;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable.Moq;

namespace Application.UnitTests.Features.UserTickets
{
    public class UserTicketServiceTests
    {
        private readonly Mock<IRepository<UserTicket, long>> _userTicketRepo = new();
        private readonly Mock<IRepository<TicketPlanPrice, long>> _planPriceRepo = new();
        private readonly Mock<IRepository<Order, long>> _orderRepo = new();
        private readonly Mock<IRepository<Wallet, long>> _walletRepo = new();
        private readonly Mock<IRepository<WalletTransaction, long>> _walletTxnRepo = new();

        private readonly Mock<IUserTicketRepository> _userTicketRepository = new();
        private readonly Mock<IVoucherRepository> _voucherRepository = new();
        private readonly Mock<IVoucherUsageRepository> _voucherUsageRepository = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private readonly IMapper _mapper;

        public UserTicketServiceTests()
        {
            var cfg = new MapperConfiguration(x =>
            {
                x.CreateMap<UserTicket, UserTicketDTO>();
                x.CreateMap<TicketPlan, UserTicketPlanDTO>();
                x.CreateMap<TicketPlanPrice, UserTicketPlanPriceDTO>();
            });
            _mapper = cfg.CreateMapper();
        }

        private UserTicketService CreateService()
        {
            return new UserTicketService(
                _userTicketRepo.Object,
                _planPriceRepo.Object,
                _orderRepo.Object,
                _walletRepo.Object,
                _walletTxnRepo.Object,
                _uow.Object,
                null!,
                _mapper,
                _userTicketRepository.Object,
                _voucherRepository.Object,
                _voucherUsageRepository.Object
            );
        }


        [Fact]
        public async Task PurchaseTicket_Success_NoVoucher_Subscription()
        {
            var svc = CreateService();
            long userId = 1;

            var planPrice = new TicketPlanPrice
            {
                Id = 10,
                Price = 100000,
                ValidityDays = 30,
                ActivationMode = PlanActivationMode.INSTANT,
                Plan = new TicketPlan { Name = "Monthly Pass", Type = "Month" }
            };

            var wallet = new Wallet { Id = 5, UserId = userId, Balance = 200000, Status = "Active" };

            _planPriceRepo.Setup(x => x.Query())
                .Returns(new List<TicketPlanPrice> { planPrice }.AsQueryable().BuildMock());

            _walletRepo.Setup(x => x.Query())
                .Returns(new List<Wallet> { wallet }.AsQueryable().BuildMock());

            _userTicketRepository.Setup(x => x.AddAsync(It.IsAny<UserTicket>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _walletTxnRepo.Setup(x => x.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = await svc.PurchaseTicketAsync(
                userId,
                new PurchaseTicketRequestDTO { PlanPriceId = 10 },
                CancellationToken.None
            );

            Assert.NotNull(dto);
            Assert.Equal("Active", dto.Status);
            Assert.Equal(100000, dto.PurchasedPrice);
            Assert.Equal(100000, wallet.Balance);
        }

        [Fact]
        public async Task PurchaseTicket_Fail_InsufficientBalance()
        {
            var svc = CreateService();
            long userId = 1;

            var planPrice = new TicketPlanPrice
            {
                Id = 10,
                Price = 200000,
                ValidityDays = 30,
                ActivationMode = PlanActivationMode.INSTANT,
                Plan = new TicketPlan { Name = "Pass", Type = "Month" }
            };

            var wallet = new Wallet { Id = 5, UserId = userId, Balance = 50000, Status = "Active" };

            _planPriceRepo.Setup(x => x.Query())
                .Returns(new List<TicketPlanPrice> { planPrice }.AsQueryable().BuildMock());

            _walletRepo.Setup(x => x.Query())
                .Returns(new List<Wallet> { wallet }.AsQueryable().BuildMock());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.PurchaseTicketAsync(
                    userId,
                    new PurchaseTicketRequestDTO { PlanPriceId = 10 },
                    CancellationToken.None
                )
            );
        }

        [Fact]
        public async Task PurchaseTicket_Success_WithVoucher_Percentage()
        {
            var svc = CreateService();
            long userId = 1;

            var planPrice = new TicketPlanPrice
            {
                Id = 10,
                Price = 100000,
                ValidityDays = 30,
                ActivationMode = PlanActivationMode.INSTANT,
                Plan = new TicketPlan { Name = "Pass", Type = "Month" }
            };

            var wallet = new Wallet { Id = 5, UserId = userId, Balance = 200000, Status = "Active" };

            var voucher = new Voucher
            {
                Id = 99,
                Code = "SALE50",
                Value = 50,
                IsPercentage = true,
                Status = VoucherStatus.Active,
                StartDate = DateTimeOffset.UtcNow.AddDays(-1),
                EndDate = DateTimeOffset.UtcNow.AddDays(1),
            };

            _planPriceRepo.Setup(x => x.Query())
                .Returns(new List<TicketPlanPrice> { planPrice }.AsQueryable().BuildMock());

            _walletRepo.Setup(x => x.Query())
                .Returns(new List<Wallet> { wallet }.AsQueryable().BuildMock());

            _voucherRepository.Setup(x => x.Query())
                .Returns(new List<Voucher> { voucher }.AsQueryable().BuildMock());

            _voucherUsageRepository.Setup(x => x.Query())
                .Returns(new List<VoucherUsage>().AsQueryable().BuildMock());

            _orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _walletTxnRepo.Setup(x => x.AddAsync(It.IsAny<WalletTransaction>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _userTicketRepository.Setup(x => x.AddAsync(It.IsAny<UserTicket>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _voucherUsageRepository.Setup(x => x.AddAsync(It.IsAny<VoucherUsage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var dto = await svc.PurchaseTicketAsync(
                userId,
                new PurchaseTicketRequestDTO { PlanPriceId = 10, VoucherCode = "SALE50" },
                CancellationToken.None
            );

            Assert.NotNull(dto);
            Assert.Equal(50000, dto.PurchasedPrice);
            Assert.Equal("Active", dto.Status);
        }

        [Fact]
        public async Task PurchaseTicket_Fail_VoucherExpired()
        {
            var svc = CreateService();
            long userId = 1;

            var planPrice = new TicketPlanPrice
            {
                Id = 10,
                Price = 100000,
                ValidityDays = 30,
                ActivationMode = PlanActivationMode.INSTANT,
                Plan = new TicketPlan()
            };

            var wallet = new Wallet { Id = 5, UserId = userId, Balance = 200000, Status = "Active" };

            var voucher = new Voucher
            {
                Id = 99,
                Code = "OLD",
                Value = 50,
                IsPercentage = true,
                Status = VoucherStatus.Active,
                StartDate = DateTimeOffset.UtcNow.AddDays(-5),
                EndDate = DateTimeOffset.UtcNow.AddDays(-1)
            };

            _planPriceRepo.Setup(x => x.Query())
                .Returns(new List<TicketPlanPrice> { planPrice }.AsQueryable().BuildMock());

            _walletRepo.Setup(x => x.Query())
                .Returns(new List<Wallet> { wallet }.AsQueryable().BuildMock());

            _voucherRepository.Setup(x => x.Query())
                .Returns(new List<Voucher> { voucher }.AsQueryable().BuildMock());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                svc.PurchaseTicketAsync(
                    userId,
                    new PurchaseTicketRequestDTO { PlanPriceId = 10, VoucherCode = "OLD" },
                    CancellationToken.None
                )
            );
        }

        [Fact]
        public async Task GetMyActiveTickets_Returns_ActiveOnly()
        {
            var svc = CreateService();
            long userId = 1;

            var tickets = new List<UserTicket>
            {
                new UserTicket
                {
                    Id = 1, UserId = userId, Status = "Active", ValidTo = DateTimeOffset.UtcNow.AddDays(1),
                    PlanPrice = new TicketPlanPrice { Plan = new TicketPlan { Name = "Pass" } }
                },
                new UserTicket
                {
                    Id = 2, UserId = userId, Status = "Expired", ValidTo = DateTimeOffset.UtcNow.AddDays(-1),
                    PlanPrice = new TicketPlanPrice { Plan = new TicketPlan { Name = "Pass" } }
                }
            };

            _userTicketRepository.Setup(x => x.Query())
                .Returns(tickets.AsQueryable().BuildMock());

            var result = await svc.GetMyActiveTicketsAsync(userId, CancellationToken.None);

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Active", result[0].Status);
        }
    }
}
