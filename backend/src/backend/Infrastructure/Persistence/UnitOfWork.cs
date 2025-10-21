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
        public UnitOfWork(HolaBikeContext context) => _context = context;
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
