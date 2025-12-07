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
        Task<List<UserTicketPlanDTO>> GetTicketMarketAsync(string? vehicleType, CancellationToken ct);
        Task<UserTicketDTO?> PurchaseTicketAsync(long? userId, PurchaseTicketRequestDTO request, CancellationToken ct);
        Task<List<UserTicketDTO>?> GetMyActiveTicketsAsync(long? userId, CancellationToken ct);
        Task<UserTicketDTO?> GetIdByUserIdAsync(long? userId, long ticketId, CancellationToken ct);
        Task<PreviewTicketPriceDTO> PreviewTicketPriceAsync(long? userId, PreviewTicketRequestDTO request, CancellationToken ct);
    }
}
