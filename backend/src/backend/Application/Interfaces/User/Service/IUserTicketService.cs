using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;

namespace Application.Interfaces.User.Service
{
    public interface IUserTicketService : IService<Domain.Entities.UserTicket, UserTicketDTO, long>
    {
        /// <summary>
        /// Lấy danh sách các vé người dùng có thể mua trên thị trường. 
        /// </summary>
        Task<List<UserTicketPlanDTO>> GetTicketMarketAsync(string? vehicleType, CancellationToken ct);
        /// <summary>
        /// Mua một vé mới. Tích hợp trừ tiền từ ví.
        /// </summary>
        Task<UserTicketDTO?> PurchaseTicketAsync(long? userId, PurchaseTicketRequestDTO request, CancellationToken ct);

        /// <summary>
        /// Lấy danh sách các vé (chưa hết hạn) của một người dùng.
        /// </summary>
        Task<List<UserTicketDTO>?> GetMyActiveTicketsAsync(long? userId, CancellationToken ct);

        Task<UserTicketDTO?> GetIdByUserIdAsync(long? userId, long ticketId, CancellationToken ct);
    }
}
