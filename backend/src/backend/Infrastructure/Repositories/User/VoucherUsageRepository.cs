using Application.Interfaces.Staff.Repository;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class VoucherUsageRepository : BaseRepository<Domain.Entities.VoucherUsage, long>, IVoucherUsageRepository
    {
        public VoucherUsageRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
