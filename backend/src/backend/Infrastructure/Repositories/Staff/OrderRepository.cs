using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staff
{
    public class OrderRepository : BaseRepository<Order, long>, IOrderRepository
    {
        public OrderRepository(HolaBikeContext context) : base(context)
        {

        }
        public Task<Order?> GetByOrderNoAsync(string orderNo, CancellationToken ct = default)
        {
            return Query()
                .FirstOrDefaultAsync(o => o.OrderNo == orderNo, ct);
        }

    }
}
