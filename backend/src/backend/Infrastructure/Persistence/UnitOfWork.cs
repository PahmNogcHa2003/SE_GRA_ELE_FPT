using Application.Interfaces;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    // Infrastructure/Persistence/UnitOfWork.cs
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HolaBikeContext _context;
        private IPaymentRepository? _payments;
        private IOrderRepository? _orders;
        private IWalletRepository? _wallets;
        private IWalletDebtRepository? _walletDebts;
        private IWalletTransactionRepository? _walletTransactions;
        public UnitOfWork(HolaBikeContext context) => _context = context;
        public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
        public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
        public IWalletDebtRepository WalletDebts => _walletDebts ??= new WalletDebtRepository(_context);
        public IWalletTransactionRepository WalletTransactions => _walletTransactions ??= new WalletTransactionRepository(_context);
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
