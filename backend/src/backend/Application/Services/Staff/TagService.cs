using Application.DTOs;
using Application.DTOs.Tag;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Staff
{
    public class TagService : GenericService<Tag, TagDTO, long>, ITagService
    {
        public TagService(IRepository<Tag, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }

        /// <summary>
        /// Overrides the generic search logic.
        /// Searches within the Name and Location properties of a Station.
        /// </summary>
        protected override IQueryable<Tag> ApplySearch(IQueryable<Tag> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return query;
            }

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(s =>
                s.Name.ToLower().Contains(lowerCaseSearchQuery)
            );
        }

        /// <summary>
        /// Ghi đè logic sắp xếp.
        /// Chỉ xử lý các quy tắc sắp xếp đặc biệt dành riêng cho Station.
        /// Các trường hợp còn lại sẽ được chuyển về cho lớp GenericService xử lý.
        /// </summary>
        protected override IQueryable<Tag> ApplySort(IQueryable<Tag> query, string? sortOrder)
        {
            var lowerSortOrder = sortOrder?.Trim().ToLower();

            switch (lowerSortOrder)
            {
                default:
                    return base.ApplySort(query, sortOrder);
            }
        }
    }
}
