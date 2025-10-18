using Application.Interfaces;
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