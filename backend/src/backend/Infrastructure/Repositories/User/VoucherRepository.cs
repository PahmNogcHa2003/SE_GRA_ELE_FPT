using Application.Interfaces.Staff.Repository;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class VoucherRepository : BaseRepository<Domain.Entities.Voucher, long>, IVoucherRepository
    {
        public VoucherRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
