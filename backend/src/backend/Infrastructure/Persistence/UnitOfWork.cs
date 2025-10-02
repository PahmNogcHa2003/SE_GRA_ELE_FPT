using Application.Interfaces;
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
    }

}
