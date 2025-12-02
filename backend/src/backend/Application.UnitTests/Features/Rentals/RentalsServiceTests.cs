//using Application.DTOs.Rental;
//using Application.Exceptions;
//using Application.Interfaces;
//using Application.Interfaces.Staff.Repository;
//using Application.Interfaces.User.Repository;
//using Application.Services.User;
//using Domain.Entities;
//using Domain.Enums;
//using Infrastructure.Persistence;
//using Infrastructure.Repositories.User;
//using Infrastructure.Repositories.Staff; // <-- Thêm namespace repo Staff (nếu có)
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace Application.UnitTests.Features.Rentals
//{
//    public class RentalsServiceTests : IDisposable
//    {
//        // 1. Chỉ mock các dependency KHÔNG phải repository
//        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor = new();
//        private readonly Mock<ILogger<RentalsService>> _mockLogger = new();

//        // 2. Sử dụng CONTEXT và REPOSITORY "THẬT"
//        private readonly HolaBikeContext _context;
//        private readonly IStationsRepository _stationRepo; // Dùng interface
//        private readonly IVehicleRepository _vehicleRepo;
//        private readonly IUserTicketRepository _userTicketRepo;
//        private readonly IRentalsRepository _rentalRepo;
//        private readonly IBookingTicketRepository _bookingTicketRepo;
//        private readonly IUnitOfWork _uow;

//        private readonly RentalsService _service;

//        // 3. Hàm khởi tạo (Constructor)
//        public RentalsServiceTests()
//        {
//            // Tạo DbContext InMemory
//            var options = new DbContextOptionsBuilder<HolaBikeContext>()
//                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                .Options;

//            _context = new HolaBikeContext(options);

//            // SỬA Ở ĐÂY: Khởi tạo TẤT CẢ repository "THẬT"
//            // (Giả định tên class của bạn là StationsRepository, VehicleRepository...)
//            _stationRepo = new StationsRepository(_context);
//            _vehicleRepo = new VehicleRepository(_context);
//            _userTicketRepo = new UserTicketRepository(_context);
//            _rentalRepo = new RentalsRepository(_context);
//            _bookingTicketRepo = new BookingTicketRepository(_context);
//            _uow = new UnitOfWork(_context);

//            // SỬA Ở ĐÂY: Truyền TẤT CẢ repo thật vào service
//            _service = new RentalsService(
//                _stationRepo,         // <-- Repo thật
//                _vehicleRepo,         // <-- Repo thật
//                _userTicketRepo,      // <-- Repo thật
//                _rentalRepo,          // <-- Repo thật
//                _bookingTicketRepo,   // <-- Repo thật
//                _uow,
//                _mockHttpContextAccessor.Object,
//                _mockLogger.Object
//            );

//            // Setup mock cơ bản
//            SetupHttpContextWithUserId(1);
//        }

//        private void SetupHttpContextWithUserId(long userId)
//        {
//            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
//            {
//                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
//            }, "mock"));

//            _mockHttpContextAccessor.Setup(a => a.HttpContext)
//                .Returns(new DefaultHttpContext { User = claimsPrincipal });
//        }

//        // 4. Hàm dọn dẹp sau mỗi test
//        public void Dispose()
//        {
//            _context.Database.EnsureDeleted();
//            _context.Dispose();
//        }

//        // 5. Các Test Case
//        //[Fact]
//        //public async Task CreateRentalAsync_ShouldCreateRental_WhenValidData()
//        //{
//        //    // Arrange
//        //    // SỬA Ở ĐÂY: "Seed" data thay vì mock
//        //    var category = new CategoriesVehicle { Id = 1, Name = "Xe đạp thường" };
//        //    var vehicle = new Vehicle
//        //    {
//        //        Id = 1,
//        //        Status = VehicleStatus.Available,
//        //        BikeCode = "TEST-001",
//        //        CategoryId = 1,
//        //        Category = category,
//        //        StationId = 5 // Xe đang ở trạm số 5
//        //    };
//        //    _context.CategoriesVehicles.Add(category);
//        //    _context.Vehicles.Add(vehicle);
//        //    await _context.SaveChangesAsync();

//        //    var createRentalDTO = new CreateRentalDTO { VehicleId = 1, UserTicketId = 10 };

//        //    // (Không cần mock _mockVehicleRepo nữa)

//        //    // Act
//        //    var result = await _service.CreateRentalAsync(createRentalDTO);

//        //    // Assert
//        //    Assert.True(result);
//        //    Assert.Equal(1, _context.Rentals.Count());
//        //    var rentalInDb = await _context.Rentals.FirstAsync();
//        //    Assert.Equal(1, rentalInDb.UserId);
//        //    Assert.Equal(1, rentalInDb.VehicleId);
//        //    Assert.Equal("Ongoing", rentalInDb.Status);
//        //    Assert.Equal(5, rentalInDb.StartStationId); // Phải lấy đúng trạm 5
//        //}

//        [Fact]
//        public async Task CreateRentalAsync_ShouldThrowBadRequest_WhenVehicleInUse()
//        {
//            // Arrange
//            // SỬA Ở ĐÂY: "Seed" data
//            var category = new CategoriesVehicle { Id = 1, Name = "Xe đạp thường" };
//            var vehicle = new Vehicle
//            {
//                Id = 1,
//                Status = VehicleStatus.InUse, // Xe đang bận
//                BikeCode = "TEST-001",
//                CategoryId = 1,
//                Category = category,
//                StationId = 5
//            };
//            _context.CategoriesVehicles.Add(category);
//            _context.Vehicles.Add(vehicle);
//            await _context.SaveChangesAsync();

//            var createRentalDTO = new CreateRentalDTO { VehicleId = 1, UserTicketId = 10 };

//            // (Không cần mock _mockVehicleRepo nữa)

//            // Act & Assert
//            await Assert.ThrowsAsync<BadRequestException>(() => _service.CreateRentalAsync(createRentalDTO));
//        }

//        [Fact]
//        public async Task EndRentalAsync_ShouldEndRental_WhenValidData()
//        {
//            // Arrange
//            // 1. "SEED" DATA: Thêm TẤT CẢ data cần thiết
//            var category = new CategoriesVehicle { Id = 1, Name = "Xe đạp thường" };
//            var vehicle = new Vehicle
//            {
//                Id = 1,
//                Status = VehicleStatus.InUse,
//                StationId = null,
//                BikeCode = "TEST-001",
//                CategoryId = 1,
//                Category = category // Thêm category
//            };
//            var station = new Station
//            {
//                Id = 99,
//                Name = "Trạm Trả Xe",
//                IsActive = true,
//                Lat = 21.013590m,
//                Lng = 105.526310m
//            };
//            var userTicket = new UserTicket
//            {
//                Id = 10, // Khớp với UserTicketId
//                UserId = 1, // Khớp với UserId
//                Status = "Active",
//                RemainingRides = 5 // Sẽ bị trừ 1
//            };
//            var bookingTicket = new BookingTicket { Id = 1, RentalId = 1, UserTicketId = 10 };
//            var ongoingRental = new Rental
//            {
//                Id = 1,
//                UserId = 1,
//                VehicleId = 1,
//                StartStationId = 1,
//                Status = "Ongoing",
//                StartTime = DateTimeOffset.UtcNow.AddMinutes(-30),
//                BookingTickets = new List<BookingTicket> { bookingTicket }
//            };

//            // Thêm tất cả vào Context
//            _context.CategoriesVehicles.Add(category);
//            _context.Vehicles.Add(vehicle);
//            _context.Stations.Add(station);
//            _context.UserTickets.Add(userTicket);
//            _context.Rentals.Add(ongoingRental); // Booking ticket được thêm gián tiếp
//            await _context.SaveChangesAsync();

//            // =================================================================
//            // ⭐ FIX: THÊM DÒNG NÀY
//            // Ngừng theo dõi (detach) tất cả các entity đã seed ở trên.
//            // Điều này giải phóng DbContext để service có thể 'Update'
//            // các entity mới mà không bị xung đột tracking.
//            _context.ChangeTracker.Clear();
//            // =================================================================

//            var endRentalDto = new EndRentalRequestDTO
//            {
//                RentalId = 1,
//                CurrentLatitude = 21.013590,
//                CurrentLongitude = 105.526310
//            };

//            // 2. KHÔNG CẦN SETUP MOCK REPO NÀO CẢ

//            // Act
//            var result = await _service.EndRentalAsync(endRentalDto);

//            // Assert
//            Assert.True(result);

//            // Kiểm tra cuốc xe
//            // (Lưu ý: dùng FindAsync vì context đã bị clear)
//            var rentalInDb = await _context.Rentals.FindAsync(1L);
//            Assert.NotNull(rentalInDb); // Thêm kiểm tra null
//            Assert.Equal(RentalStatus.End, rentalInDb.Status);
//            Assert.Equal(99, rentalInDb.EndStationId);

//            // Kiểm tra xe
//            var vehicleInDb = await _context.Vehicles.FindAsync(1L);
//            Assert.NotNull(vehicleInDb); // Thêm kiểm tra null
//            Assert.Equal("Available", vehicleInDb.Status);
//            Assert.Equal(99, vehicleInDb.StationId);

//            // Kiểm tra vé
//            var ticketInDb = await _context.UserTickets.FindAsync(10L);
//            Assert.NotNull(ticketInDb); // Thêm kiểm tra null
//            Assert.Equal(4, ticketInDb.RemainingRides);
//        }
//    }
//}
