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

namespace Application.Services.Staff
{
    public class ManageTicketPlanSevice : GenericService<TicketPlan, TicketPlanDTO, long>, IManageTicketPlanService
    {
        public ManageTicketPlanSevice(IRepository<TicketPlan,long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
        protected override IQueryable<TicketPlan> ApplySearch(IQueryable<TicketPlan> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return query;

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();
            return query.Where(p =>
                (p.Name != null && p.Name.ToLower().Contains(lowerCaseSearchQuery)) ||
                (p.Code != null && p.Code.ToLower().Contains(lowerCaseSearchQuery))
            );
        }
    }
}
