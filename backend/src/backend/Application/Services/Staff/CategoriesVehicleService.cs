using Application.DTOs;
using Application.DTOs.CategoriesVehicle;
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
    public class CategoriesVehicleService : GenericService<CategoriesVehicle, CategoriesVehicleDTO, long>, ICategoriesVehicleService
    {
        public CategoriesVehicleService(IRepository<CategoriesVehicle, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }

        protected override IQueryable<CategoriesVehicle> ApplySearch(IQueryable<CategoriesVehicle> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return query;
            }

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(s =>
                s.Name.ToLower().Contains(lowerCaseSearchQuery) ||
                s.Description.ToLower().Contains(lowerCaseSearchQuery)
            );
        }

        protected override IQueryable<CategoriesVehicle> ApplyFilter(IQueryable<CategoriesVehicle> query, string filterField, string filterValue)
        {
            var lowerFilterField = filterField.ToLower();

            switch (lowerFilterField)
            {
                // Filter by exact Status (supports "true"/"false" and "1"/"0")
                case "isactive":
                    {
                        if (bool.TryParse(filterValue, out bool isActive))
                        {
                            return query.Where(s => s.IsActive == isActive);
                        }

                        if (filterValue == "1") return query.Where(s => s.IsActive == true);
                        if (filterValue == "0") return query.Where(s => s.IsActive == false);
                        return query;
                    }

                // Default: fall back to the base class logic (uses .Contains() for other fields)
                default:
                    return base.ApplyFilter(query, filterField, filterValue);
            }
        }

        protected override IQueryable<CategoriesVehicle> ApplySort(IQueryable<CategoriesVehicle> query, string? sortOrder)
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
