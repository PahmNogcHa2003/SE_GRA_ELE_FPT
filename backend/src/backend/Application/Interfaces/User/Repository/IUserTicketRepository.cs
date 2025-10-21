using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface IUserTicketRepository : IRepository<UserTicket, long>
    {
        /// <summary>
        /// Kiểm tra xem người dùng có gói vé dạng thuê bao (ngày/tháng) đang hoạt động hoặc chờ kích hoạt hay không.
        /// Dùng để ngăn người dùng mua trùng lặp.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="planPriceId">ID của loại vé (plan price)</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>True nếu đã tồn tại, ngược lại là false.</returns>
        Task<bool> HasExistingSubscriptionAsync(long userId, long planPriceId, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách tất cả các vé chưa hết hạn của một người dùng.
        /// Bao gồm thông tin của Plan và PlanPrice để hiển thị cho người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="ct">Cancellation Token</param>
        /// <returns>Danh sách UserTicket.</returns>
        Task<List<UserTicket>> GetActiveTicketsByUserIdAsync(long userId, CancellationToken ct = default);
    }
}
