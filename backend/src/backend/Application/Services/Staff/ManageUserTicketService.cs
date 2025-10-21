using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tickets;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Staff
{
    public class ManageUserTicketService : GenericService<UserTicket, ManageUserTicketDTO , long>, IManageUserTicketService
    {
        public ManageUserTicketService(IRepository<UserTicket, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
        // Ghi đè phương thức ApplySearch để có thể tìm kiếm theo Email người dùng
        protected override IQueryable<UserTicket> ApplySearch(IQueryable<UserTicket> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(ut =>
                (ut.User.Email != null && ut.User.Email.ToLower().Contains(lowerCaseSearchQuery)) ||
                (ut.SerialCode != null && ut.SerialCode.ToLower().Contains(lowerCaseSearchQuery))
            );
        }

        // Ghi đè query mặc định để luôn join các bảng cần thiết
        // Điều này rất quan trọng để GetPagedAsync hoạt động đúng với DTO mới
        protected override IQueryable<UserTicket> GetQueryWithIncludes()
        {
            return _repo.Query()
                        .Include(ut => ut.User)
                        .Include(ut => ut.PlanPrice)
                            .ThenInclude(pp => pp.Plan);
        }

        // Triển khai nghiệp vụ hủy vé
        public async Task<ManageUserTicketDTO> VoidTicketAsync(long ticketId, string reason, CancellationToken ct = default)
        {
            // Lấy vé từ DB, lần này cần tracking để update
            var ticket = await _repo.Query()
                .FirstOrDefaultAsync(ut => ut.Id == ticketId);

            if (ticket == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy vé với ID {ticketId}.");
            }

            if (ticket.Status == "Voided" || ticket.Status == "Expired")
            {
                throw new InvalidOperationException($"Vé này đã ở trạng thái '{ticket.Status}' và không thể hủy.");
            }

            // Thực hiện nghiệp vụ
            ticket.Status = "Voided";
            // Ghi chú: Đây là nơi bạn có thể thêm logic hoàn tiền
            // await _walletService.CreditAsync(ticket.UserId, ticket.PurchasedPrice ?? 0, $"Refund_VoidTicket_{ticket.Id}", null, CancellationToken.None);

            _repo.Update(ticket);
            await _uow.SaveChangesAsync();

            return _mapper.Map<ManageUserTicketDTO>(ticket);
        }

    }
}
