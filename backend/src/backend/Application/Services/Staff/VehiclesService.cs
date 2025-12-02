using Application.DTOs;
using Application.DTOs.Vehicle;
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
    public class VehiclesService : GenericService<Vehicle, VehicleDTO, long>, IVehicleService
    {
        public VehiclesService(IRepository<Vehicle, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }

        protected override IQueryable<Vehicle> ApplySearch(IQueryable<Vehicle> query, string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return query;
            }

            var lowerCaseSearchQuery = searchQuery.Trim().ToLower();

            return query.Where(s =>
                s.BikeCode.ToLower().Contains(lowerCaseSearchQuery) ||
                s.BatteryLevel.ToString().Contains(lowerCaseSearchQuery) 
            );
        }


        protected override IQueryable<Vehicle> ApplyFilter(IQueryable<Vehicle> query, string filterField, string filterValue)
        {
            var lowerFilterField = filterField.ToLower();

            switch (lowerFilterField)
            {
                // Filter by exact Status (supports "true"/"false" and "1"/"0")
                case "status":
                    {
                        if (!string.IsNullOrEmpty(filterValue))
                        {
                            // Normalize giá trị để tránh lỗi do chữ hoa/thường
                            var normalized = filterValue.Trim().ToLower();

                            // Chỉ lọc nếu giá trị nằm trong danh sách trạng thái hợp lệ
                            var validStatuses = new[] { "available", "inuse", "maintenance", "unavailable" };
                            if (validStatuses.Contains(normalized))
                            {
                                return query.Where(s => s.Status.ToLower() == normalized);
                            }
                        }

                        // Nếu filterValue null hoặc không hợp lệ → không lọc
                        return query;
                    }


                case "chargingstatus":
                    {
                        if (bool.TryParse(filterValue, out bool isActive))
                        {
                            return query.Where(s => s.ChargingStatus == isActive);
                        }

                        if (filterValue == "1") return query.Where(s => s.ChargingStatus == true);
                        if (filterValue == "0") return query.Where(s => s.ChargingStatus == false);
                        return query;
                    }
                // Default: fall back to the base class logic (uses .Contains() for other fields)
                default:
                    return base.ApplyFilter(query, filterField, filterValue);
            }
        }

        protected override IQueryable<Vehicle> ApplySort(IQueryable<Vehicle> query, string? sortOrder)
        {
            var lowerSortOrder = sortOrder?.Trim().ToLower();

            switch (lowerSortOrder)
            {
                default:
                    return base.ApplySort(query, sortOrder);
            }
        }

        public Task<int> GetStationVehicle(int vehicleId)
        {
            throw new NotImplementedException();
        }
    }
}
