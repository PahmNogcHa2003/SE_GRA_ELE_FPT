using Application.Interfaces;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HolaBikeContext _context;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(HolaBikeContext context)
        {
            _context = context;
        }

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

        public IDbContextTransaction BeginTransaction()
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = _context.Database.BeginTransaction();
            return _currentTransaction;
        }

        public async Task CommitAsync()
        {
            if (_currentTransaction is null)
                throw new InvalidOperationException("Cannot commit transaction. No transaction started.");

            try
            {
                await _context.SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_currentTransaction is null) return;

            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose();
                _context.Dispose();
            }
        }
    }
}