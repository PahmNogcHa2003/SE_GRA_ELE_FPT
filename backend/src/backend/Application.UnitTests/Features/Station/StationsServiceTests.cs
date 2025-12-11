using Application.DTOs.Station;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Services.Base; // GenericService là dependency
using Application.Services.User;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Staff;
using Infrastructure.Repositories.User; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Application.UnitTests.Services
{
    public class StationsServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IRepository<Station, long> _stationRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        private readonly StationsService _service;

        public StationsServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);

            _stationRepo = new BaseRepository<Station, long>(_context);
            _uow = new UnitOfWork(_context); 

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Station, StationDTO>()
                    .ForMember(d => d.VehicleAvailable, opt => opt.Ignore())
                    .ForMember(d => d.DistanceKm, opt => opt.Ignore());
            });
            _mapper = config.CreateMapper();

            _service = new StationsService(_stationRepo, _mapper, _uow);
        }

        private async Task SeedDataAsync(List<Station> stations)
        {
            foreach (var s in stations)
            {
                _context.Stations.Add(s);
            }
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear(); 
        }

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
                    IsActive = true,
                    Vehicles = new List<Vehicle>()
                            }
                };
                    }

      
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

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}