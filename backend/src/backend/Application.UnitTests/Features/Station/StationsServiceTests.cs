using Application.DTOs.Station;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Services.Base; // GenericService là dependency
using Application.Services.User;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User; // Cần Repository thật nếu có
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Application.UnitTests.Services
{
    // Sử dụng IDisposable để đảm bảo DB được dọn dẹp sau mỗi test
    public class StationsServiceTests : IDisposable
    {
        // Sử dụng DbContext và Repository THẬT
        private readonly HolaBikeContext _context;
        private readonly IRepository<Station, long> _stationRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        private readonly StationsService _service;

        public StationsServiceTests()
        {
            // 1. Setup DbContext InMemory
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);

            // 2. Setup Repository và UnitOfWork THẬT
            // (Giả định IRepository được implement bởi GenericRepository hoặc tương đương)
            // Nếu bạn có một GenericRepository:
            // _stationRepo = new GenericRepository<Station, long>(_context);
            // Dù trong StationsService nó nhận IRepository<Station, long>
            // Ở đây tôi giả định bạn có thể sử dụng một mock đơn giản cho IRepository
            // HOẶC sử dụng lại cấu trúc Repository thật nếu bạn có (tôi sẽ dùng một Mock đơn giản cho IRepository, nhưng sử dụng hàm .Query() trả về từ DB context thật)

            // **Sửa đổi dựa trên dependency:** // StationsService nhận IRepository<Station, long>
            // Ta dùng GenericRepository hoặc BaseRepository cho nó. (Giả định bạn có GenericRepository)
            _stationRepo = new BaseRepository<Station, long>(_context);
            _uow = new UnitOfWork(_context); // Giả định UnitOfWork có sẵn

            // 3. Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                // Cần đảm bảo mapping này có sẵn để ProjectToDto hoạt động
                cfg.CreateMap<Station, StationDTO>()
                    .ForMember(d => d.VehicleAvailable, opt => opt.Ignore())
                    .ForMember(d => d.DistanceKm, opt => opt.Ignore());
            });
            _mapper = config.CreateMapper();

            // 4. Khởi tạo Service
            _service = new StationsService(_stationRepo, _mapper, _uow);
        }

        // Phương thức để nạp dữ liệu mẫu vào DB
        private async Task SeedDataAsync(List<Station> stations)
        {
            foreach (var s in stations)
            {
                _context.Stations.Add(s);
            }
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear(); // Dọn dẹp để tránh lỗi tracking giữa các test
        }

        // Application.UnitTests.Services/StationsServiceTests.cs

        private List<Station> GetSampleStationsList()
        {
            return new List<Station>
    {
        new Station
        {
            Id = 1,
            Name = "Station A",
            Location = "Hanoi",
            Capacity = 10,
            Lat = 21.0m,
            Lng = 105.0m,
            IsActive = true,
            Vehicles = new List<Vehicle>
            {
                // FIX: Thêm BikeCode
                new Vehicle { BikeCode = "A-001", Status = "Available" },
                new Vehicle { BikeCode = "A-002", Status = "Unavailable" }
            }
        },
        new Station
        {
            Id = 2,
            Name = "Station B",
            Location = "HCM",
            Capacity = 5,
            Lat = 10.0m,
            Lng = 106.0m,
            IsActive = true,
            Vehicles = new List<Vehicle>
            {
                // FIX: Thêm BikeCode
                new Vehicle { BikeCode = "B-001", Status = "Available" }
            }
        },
        new Station
                {
                    Id = 3,
                    Name = "Station C",
                    Location = "Hue",
                    Capacity = 8,
                    Lat = 16.0m,
                    Lng = 107.0m,
                    // SỬA: Thay đổi IsActive từ false thành true
                    IsActive = true,
                    Vehicles = new List<Vehicle>()
                }
    };
        }

        // -------------------------------------------------------------------------
        //                              TEST CASES
        // -------------------------------------------------------------------------

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllStationsWithVehicleAvailableCount()
        {
            // Arrange: Seed data
            await SeedDataAsync(GetSampleStationsList());

            // Act: Service sẽ truy vấn DbContext InMemory
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
            var stationA = result.First(s => s.Id == 1);

            // Kiểm tra logic đếm xe có sẵn (xe 1 có 1 xe Available)
            Assert.Equal(1, stationA.VehicleAvailable);
        }

        [Fact]
        public async Task GetNearbyPagedAsync_ShouldReturnStationsWithinRadius()
        {
            // Arrange: Seed data
            await SeedDataAsync(GetSampleStationsList());

            double lat = 21.0;
            double lng = 105.0;
            double radiusKm = 2000;
            int page = 1;
            int pageSize = 10;

            // Act
            var pagedResult = await _service.GetNearbyPagedAsync(lat, lng, radiusKm, page, pageSize);

            // Assert
            Assert.Equal(3, pagedResult.Items.Count()); // 3 trạm nằm trong bán kính lớn
            Assert.Contains(pagedResult.Items, s => s.Name == "Station A");
            pagedResult.Items.First().Name.Should().Be("Station A"); // Trạm gần nhất
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnFilteredResults()
        {
            // Arrange: Seed data
            await SeedDataAsync(GetSampleStationsList());

            // Act
            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, searchQuery: "Station B");

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Station B", result.Items.First().Name);
        }

        [Fact]
        public async Task GetPagedAsync_WithFilter_ShouldReturnFilteredResults()
        {
            // Arrange: Seed data
            await SeedDataAsync(GetSampleStationsList());

            // Act
            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, filterField: "isActive", filterValue: "true");

            // Assert
            Assert.Equal(3, result.Items.Count());
            Assert.All(result.Items, s => Assert.True(s.IsActive));
        }

        // -------------------------------------------------------------------------
        //                              HÀM DỌN DẸP
        // -------------------------------------------------------------------------

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}