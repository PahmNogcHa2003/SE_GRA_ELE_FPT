using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    // Application/Interfaces/IUnitOfWork.cs
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        IDbContextTransaction BeginTransaction();
        Task CommitAsync();
        Task RollbackAsync();
    }

}
