using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using Domain.Entities;

namespace Application.Services.Staff
{
    public class TicketPlanPriceService : GenericService<Domain.Entities.TicketPlanPrice, TicketPlanPriceDTO, long>, ITicketPlanPriceService
    {
        public TicketPlanPriceService(
            IRepository<Domain.Entities.TicketPlanPrice, long> repo,
            AutoMapper.IMapper mapper,
            Interfaces.IUnitOfWork uow
        ) : base(repo, mapper, uow)
        {
        }
        protected override IQueryable<TicketPlanPrice> ApplySearch(IQueryable<TicketPlanPrice> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();
            return query.Where(p =>
                (p.VehicleType != null && p.VehicleType.ToLower().Contains(lowerCaseSearchQuery)) ||
                 p.PlanId.ToString().Contains(lowerCaseSearchQuery) ||
                (p.Plan != null && p.Plan.Name.ToLower().Contains(lowerCaseSearchQuery)));
        }
        protected override IQueryable<TicketPlanPrice> ApplyFilter(
        IQueryable<TicketPlanPrice> query,
        string filterField,
        string filterValue)
        {
            if (string.IsNullOrWhiteSpace(filterField) || string.IsNullOrWhiteSpace(filterValue))
                return query;

            // ✅ Lọc theo VehicleType: chỉ nhận "bike" hoặc "ebike"
            if (filterField.Equals("vehicletype", StringComparison.OrdinalIgnoreCase))
            {
                var lower = filterValue.Trim().ToLower();

                if (lower == "bike")
                {
                    return query.Where(p => p.VehicleType.ToLower() == "bike");
                }
                else if (lower == "ebike")
                {
                    return query.Where(p => p.VehicleType.ToLower() == "ebike");
                }
                else
                {
                    return query;
                }
            }
            return base.ApplyFilter(query, filterField, filterValue);
        }
    }
}
