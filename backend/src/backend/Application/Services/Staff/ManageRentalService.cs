using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Application.Common;
using Application.DTOs.Rental.Manage;

namespace Application.Services.Staff
{
    public class ManageRentalService : IManageRentalsService
    {
        private readonly IRentalsRepository _rentalRepo;
        private readonly IRentalHistoryRepository _rentalHistoryRepo;
        private readonly IBookingTicketRepository _bookingTicketRepo;
        private readonly IUserTicketRepository _userTicketRepo;
        private readonly IWalletDebtRepository _walletDebtRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IUnitOfWork _uow;

        public ManageRentalService(
            IRentalsRepository rentalRepo,
            IRentalHistoryRepository rentalHistoryRepo,
            IBookingTicketRepository bookingTicketRepo,
            IUserTicketRepository userTicketRepo,
            IWalletDebtRepository walletDebtRepo,
            IOrderRepository orderRepo,
            IUnitOfWork uow)
        {
            _rentalRepo = rentalRepo;
            _rentalHistoryRepo = rentalHistoryRepo;
            _bookingTicketRepo = bookingTicketRepo;
            _userTicketRepo = userTicketRepo;
            _walletDebtRepo = walletDebtRepo;
            _orderRepo = orderRepo;
            _uow = uow;
        }
        public async Task<PagedResult<RentalListDTO>> GetPagedAsync(
            int page,
            int pageSize,
            RentalFilterDTO filter,
            CancellationToken ct = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = _rentalRepo.Query()
                .Include(r => r.User).ThenInclude(u => u.UserProfile)
                .Include(r => r.Vehicle).ThenInclude(v => v.Category)
                .Include(r => r.StartStation)
                .Include(r => r.EndStation)
                .Include(r => r.BookingTickets)
                .AsNoTracking();

            // ----- Apply filter -----
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(r => r.Status == filter.Status);
            }

            if (filter.UserId.HasValue)
                query = query.Where(r => r.UserId == filter.UserId.Value);

            if (filter.VehicleId.HasValue)
                query = query.Where(r => r.VehicleId == filter.VehicleId.Value);

            if (filter.StartStationId.HasValue)
                query = query.Where(r => r.StartStationId == filter.StartStationId.Value);

            if (filter.EndStationId.HasValue)
                query = query.Where(r => r.EndStationId == filter.EndStationId.Value);

            if (filter.FromStartTimeUtc.HasValue)
                query = query.Where(r => r.StartTime >= filter.FromStartTimeUtc.Value);

            if (filter.ToStartTimeUtc.HasValue)
                query = query.Where(r => r.StartTime <= filter.ToStartTimeUtc.Value);

            if (filter.FromEndTimeUtc.HasValue)
                query = query.Where(r => r.EndTime.HasValue && r.EndTime.Value >= filter.FromEndTimeUtc.Value);

            if (filter.ToEndTimeUtc.HasValue)
                query = query.Where(r => r.EndTime.HasValue && r.EndTime.Value <= filter.ToEndTimeUtc.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var kw = filter.Keyword.Trim().ToLower();
                query = query.Where(r =>
                    (r.Vehicle.BikeCode != null && r.Vehicle.BikeCode.ToLower().Contains(kw)) ||
                    (r.User.UserProfile.FullName != null && r.User.UserProfile.FullName.ToLower().Contains(kw)) ||
                    (r.User.Email != null && r.User.Email.ToLower().Contains(kw)));
            }

            // ----- Sort: mặc định mới nhất ở trên -----
            query = query.OrderByDescending(r => r.StartTime);

            // ----- Paging -----
            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            var dtoList = new List<RentalListDTO>();

            foreach (var r in items)
            {
                int? durationMinutes = null;
                if (r.EndTime.HasValue)
                    durationMinutes = (int)(r.EndTime.Value - r.StartTime).TotalMinutes;

                var booking = r.BookingTickets
                    .OrderByDescending(b => b.AppliedAt)
                    .FirstOrDefault();

                decimal? overFee = booking?.OverusedFee;

                dtoList.Add(new RentalListDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserFullName = r.User.UserProfile?.FullName,

                    BikeCode = r.Vehicle?.BikeCode,
                    VehicleType = r.Vehicle?.Category?.Name,

                    StartStationName = r.StartStation?.Name,
                    EndStationName = r.EndStation?.Name,

                    StartTimeUtc = r.StartTime,
                    EndTimeUtc = r.EndTime,
                    Status = r.Status,
                    DurationMinutes = durationMinutes,

                    OverusedFee = overFee,
                });
            }

            return new PagedResult<RentalListDTO>(dtoList, totalCount, page, pageSize);

        }

        public async Task<RentalDetailDTO> GetDetailAsync(long id, CancellationToken ct = default)
        {
            var rental = await _rentalRepo.Query()
                .Include(r => r.User).ThenInclude(u => u.UserProfile)
                .Include(r => r.Vehicle).ThenInclude(v => v.Category)
                .Include(r => r.StartStation)
                .Include(r => r.EndStation)
                .Include(r => r.BookingTickets)
                    .ThenInclude(bt => bt.UserTicket)
                        .ThenInclude(ut => ut.PlanPrice)
                            .ThenInclude(pp => pp.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, ct)
                ?? throw new KeyNotFoundException($"Rental id={id} không tồn tại.");

            var booking = rental.BookingTickets
                .OrderByDescending(bt => bt.AppliedAt)
                .FirstOrDefault();

            UserTicket? userTicket = booking?.UserTicket;
            TicketPlanPrice? planPrice = userTicket?.PlanPrice;

            int? durationMinutes = null;
            if (rental.EndTime.HasValue)
                durationMinutes = (int)(rental.EndTime.Value - rental.StartTime).TotalMinutes;

            // RentalHistory
            var histories = await _rentalHistoryRepo.Query()
                .Where(h => h.RentalId == rental.Id)
                .AsNoTracking()
                .OrderBy(h => h.Timestamp)
                .ToListAsync(ct);

            var historyDtos = histories.Select(h => new RentalHistoryItemDTO
            {
                Id = h.Id,
                ActionType = h.ActionType,
                TimestampUtc = h.Timestamp,
                Description = h.Description,
                DistanceKm = h.DistanceMeters.HasValue ? h.DistanceMeters.Value / 1000.0 : null
            }).ToList();

            // Tìm Order + Debt liên quan đến overtime (nếu có)
            WalletDebt? debt = null;
            Order? overtimeOrder = null;

            if (booking?.OverusedFee is not null && booking.OverusedFee > 0)
            {
                // Tìm order overtime mới nhất cho user trong khoảng time này
                overtimeOrder = await _orderRepo.Query()
                    .Where(o => o.UserId == rental.UserId && o.OrderType == OrderTypeConstants.RentalOvertime)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync(ct);

                if (overtimeOrder != null)
                {
                    debt = await _walletDebtRepo.Query()
                        .Where(d => d.OrderId == overtimeOrder.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(ct);
                }
            }

            double? distanceKm = historyDtos
                .Where(h => h.DistanceKm.HasValue)
                .Select(h => h.DistanceKm!.Value)
                .DefaultIfEmpty(0)
                .Max();

            var dto = new RentalDetailDTO
            {
                Id = rental.Id,
                UserId = rental.UserId,
                UserFullName = rental.User.UserProfile?.FullName,
                UserEmail = rental.User.Email,
                UserPhone = rental.User.PhoneNumber,

                BikeCode = rental.Vehicle?.BikeCode,
                VehicleType = rental.Vehicle?.Category?.Name,

                StartStationName = rental.StartStation?.Name,
                EndStationName = rental.EndStation?.Name,

                StartTimeUtc = rental.StartTime,
                EndTimeUtc = rental.EndTime,
                DurationMinutes = durationMinutes,

                UserTicketId = userTicket?.Id,
                TicketPlanName = planPrice?.Plan?.Name,
                TicketType = planPrice?.Plan?.Type,
                TicketPlanPrice = planPrice?.Price,

                OverusedMinutes = booking?.OverusedMinutes,
                OverusedFee = booking?.OverusedFee,

                DistanceKm = distanceKm,

                Status = rental.Status,
                CreatedAt = rental.CreatedAt,

                OvertimeOrderId = overtimeOrder?.Id,
                OvertimeOrderNo = overtimeOrder?.OrderNo,
                OvertimeOrderStatus = overtimeOrder?.Status,
                OvertimeDebtAmount = debt?.Amount,
                OvertimeDebtRemaining = debt?.Remaining,
                OvertimeDebtStatus = debt?.Status,

                History = historyDtos
            };

            return dto;
        }
    }
}
