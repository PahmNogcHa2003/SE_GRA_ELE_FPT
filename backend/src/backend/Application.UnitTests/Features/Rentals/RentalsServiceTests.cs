using Application.DTOs.Rental;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.User;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Features.Rentals
{
    public class RentalsServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;

        private readonly IStationsRepository _stationRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IUserTicketRepository _userTicketRepo;
        private readonly IRentalsRepository _rentalRepo;
        private readonly IBookingTicketRepository _bookingTicketRepo;
        private readonly IRentalHistoryRepository _rentalHistoryRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletDebtRepository _walletDebtRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IRepository<TicketPlanPrice, long> _ticketPlanRepo;

        private readonly IUnitOfWork _uow;

        private readonly Mock<IQuestService> _mockQuestService = new();
        private readonly Mock<IUserLifetimeStatsService> _mockUserLifetimeService = new();
        private readonly Mock<IHttpContextAccessor> _mockHttpContext = new();
        private readonly Mock<ILogger<RentalsService>> _mockLogger = new();

        private readonly RentalsService _service;

        public RentalsServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);

            _stationRepo = new StationsRepository(_context);
            _vehicleRepo = new VehicleRepository(_context);
            _userTicketRepo = new UserTicketRepository(_context);
            _rentalRepo = new RentalsRepository(_context);
            _bookingTicketRepo = new BookingTicketRepository(_context);
            _rentalHistoryRepo = new RentalHistoryRepository(_context);
            _walletRepo = new WalletRepository(_context);
            _walletDebtRepo = new WalletDebtRepository(_context);
            _orderRepo = new OrderRepository(_context);

            // FIX: dùng BaseRepository thay vì TicketPlanPriceService
            _ticketPlanRepo = new BaseRepository<TicketPlanPrice, long>(_context);

            _uow = new UnitOfWork(_context);

            _service = new RentalsService(
                _stationRepo,
                _vehicleRepo,
                _userTicketRepo,
                _rentalRepo,
                _bookingTicketRepo,
                _rentalHistoryRepo,
                _walletRepo,
                _walletDebtRepo,
                _orderRepo,
                _ticketPlanRepo,
                _uow,
                _mockHttpContext.Object,
                _mockQuestService.Object,
                _mockUserLifetimeService.Object,
                _mockLogger.Object
            );

            SetUser(1);
        }

        private void SetUser(long id)
        {
            var claims = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()) },
                    "mock"
                )
            );

            _mockHttpContext.Setup(x => x.HttpContext)
                .Returns(new DefaultHttpContext { User = claims });
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldThrowBadRequest_WhenVehicleInUse()
        {
            var category = new CategoriesVehicle { Id = 1, Name = "Xe đạp" };
            var vehicle = new Vehicle
            {
                Id = 1,
                Status = VehicleStatus.InUse,
                BikeCode = "X01",
                CategoryId = 1,
                Category = category,
                StationId = 5
            };

            _context.CategoriesVehicles.Add(category);
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var dto = new CreateRentalDTO { VehicleId = 1, UserTicketId = 10 };

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _service.CreateRentalAsync(dto));
        }

        [Fact]
        public async Task EndRentalAsync_ShouldEndRental_WhenValid()
        {
            var category = new CategoriesVehicle { Id = 1, Name = "Normal" };
            var station = new Station
            {
                Id = 99,
                Name = "Trạm trả xe",
                IsActive = true,
                Lat = 21.013590m,
                Lng = 105.526310m
            };
            var vehicle = new Vehicle
            {
                Id = 1,
                Status = VehicleStatus.InUse,
                CategoryId = 1,
                Category = category,
                StationId = null
            };
            var ticket = new UserTicket
            {
                Id = 10,
                UserId = 1,
                Status = "Active",
                RemainingRides = 5
            };
            var rental = new Rental
            {
                Id = 1,
                UserId = 1,
                VehicleId = 1,
                StartStationId = 1,
                Status = RentalStatus.Ongoing.ToString(),
                StartTime = DateTimeOffset.UtcNow.AddMinutes(-30)
            };
            var booking = new BookingTicket
            {
                Id = 1,
                RentalId = 1,
                UserTicketId = 10
            };

            _context.CategoriesVehicles.Add(category);
            _context.Stations.Add(station);
            _context.Vehicles.Add(vehicle);
            _context.UserTickets.Add(ticket);
            _context.Rentals.Add(rental);
            _context.BookingTickets.Add(booking);

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var request = new EndRentalRequestDTO
            {
                RentalId = 1,
                CurrentLatitude = 21.013590,
                CurrentLongitude = 105.526310
            };

            var result = await _service.EndRentalAsync(request);

            Assert.True(result);

            var rentalDb = await _context.Rentals.FindAsync(1L);
            Assert.Equal(RentalStatus.End.ToString(), rentalDb.Status);
            Assert.Equal(99, rentalDb.EndStationId);

            var vehicleDb = await _context.Vehicles.FindAsync(1L);
            Assert.Equal(VehicleStatus.Available, vehicleDb.Status);
            Assert.Equal(99, vehicleDb.StationId);

            var ticketDb = await _context.UserTickets.FindAsync(10L);
            Assert.Equal(4, ticketDb.RemainingRides);
        }
    }
}
