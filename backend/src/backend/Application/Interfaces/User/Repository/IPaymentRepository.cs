using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface IPaymentRepository : IRepository<Payment, long>
    {
        Task<Payment?> GetPaymentWithOrderAndUserAsync(long paymentId, CancellationToken cancellationToken);
        Task<Payment?> GetByOrderIdAsync(long orderId, CancellationToken cancellationToken);
    }
}
