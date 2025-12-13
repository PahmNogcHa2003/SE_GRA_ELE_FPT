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
        protected override IQueryable<UserTicket> ApplyFilter(IQueryable<UserTicket> query,string filterField,string filterValue)
        {
            // nếu không có filter, trả về query gốc
            if (string.IsNullOrWhiteSpace(filterField) || string.IsNullOrWhiteSpace(filterValue))
                return base.ApplyFilter(query, filterField, filterValue);

            var val = filterValue.Trim();

            switch (filterField.Trim().ToLower())
            {
                case "planname":
                case "plan_name":
                case "plan":
                    // đảm bảo join PlanPrice -> Plan đã có trong GetQueryWithIncludes()
                    query = query.Where(ut =>
                        ut.PlanPrice != null &&
                        ut.PlanPrice.Plan != null &&
                        ut.PlanPrice.Plan.Name != null &&
                        EF.Functions.Like(ut.PlanPrice.Plan.Name.ToLower(), $"%{val.ToLower()}%")
                    );
                    break;

                case "useremail":
                case "email":
                    query = query.Where(ut =>
                        ut.User != null &&
                        ut.User.Email != null &&
                        EF.Functions.Like(ut.User.Email.ToLower(), $"%{val.ToLower()}%")
                    );
                    break;

                case "serialcode":
                    query = query.Where(ut => ut.SerialCode != null && EF.Functions.Like(ut.SerialCode.ToLower(), $"%{val.ToLower()}%"));
                    break;

                default:
                    // fallback: gọi base để xử lý (nếu có implementation chung)
                    query = base.ApplyFilter(query, filterField, filterValue);
                    break;
            }

            return query;
        }

        protected override IQueryable<UserTicket> ApplySort(IQueryable<UserTicket> query,string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
            {
                // Mặc định: sort mới nhất trước
                return query.OrderByDescending(x => x.CreatedAt);
            }

            return sortOrder.ToLower() switch
            {
                "createdat_asc" => query.OrderBy(x => x.CreatedAt),
                "createdat_desc" => query.OrderByDescending(x => x.CreatedAt),

                // fallback nếu FE gửi sort lạ
                _ => query.OrderByDescending(x => x.CreatedAt)
            };
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
