using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.User
{
    public class PaymentRepository : BaseRepository<Payment,long> , IPaymentRepository
    {
        public PaymentRepository(HolaBikeContext context) : base(context){}
        public async Task<Payment?> GetPaymentWithOrderAndUserAsync(long paymentId, CancellationToken cancellationToken)
        {
            return await _dbContext.Payments
                .Include(p => p.Order)
                    .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);
        }
        public Task<Payment?> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken)
        {
            return _dbContext.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId, cancellationToken);
        }
    }
}
