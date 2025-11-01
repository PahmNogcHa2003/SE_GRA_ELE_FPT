using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.Staff.Service
{
    public interface IManageUserTicketService : IService<UserTicket, ManageUserTicketDTO, long>
    {
        /// <summary>
        /// Hủy một vé của người dùng (ví dụ: do gian lận hoặc yêu cầu hỗ trợ).
        /// </summary>
        /// <param name="ticketId">ID của vé cần hủy.</param>
        /// <param name="reason">Lý do hủy (để ghi log).</param>
        /// <returns>Thông tin vé sau khi đã hủy.</returns>
        Task<ManageUserTicketDTO> VoidTicketAsync(long ticketId, string reason, CancellationToken ct = default);
    }
}
