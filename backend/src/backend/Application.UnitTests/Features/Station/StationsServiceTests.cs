using Xunit;
using Moq;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Services.User;
using Application.Interfaces.Base;
using Application.DTOs.Station;
using Domain.Entities;
using Application.Interfaces;
using Application.UnitTests.Helpers; // chứa AsyncTestHelper

namespace Application.UnitTests.Services
{
    public class StationsServiceTests
    {
        private readonly Mock<IRepository<Station, long>> _repoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly IMapper _mapper;
        private readonly StationsService _service;

        public StationsServiceTests()
        {
            _repoMock = new Mock<IRepository<Station, long>>();
            _uowMock = new Mock<IUnitOfWork>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Station, StationDTO>()
                    .ForMember(d => d.VehicleAvailable, opt => opt.Ignore())
                    .ForMember(d => d.DistanceKm, opt => opt.Ignore());
            });
            _mapper = config.CreateMapper();

            _service = new StationsService(_repoMock.Object, _mapper, _uowMock.Object);
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
                        new Vehicle { Status = "Available" },
                        new Vehicle { Status = "Unavailable" }
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
                        new Vehicle { Status = "Available" }
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
                    IsActive = false,
                    Vehicles = new List<Vehicle>()
                }
            };
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllStationsWithVehicleAvailableCount()
        {
            _repoMock.Setup(r => r.Query()).Returns(GetSampleStationsList().AsQueryableForTest());

            var result = await _service.GetAllAsync();

            Assert.Equal(3, result.Count());
            var stationA = result.First(s => s.Id == 1);
            Assert.Equal(1, stationA.VehicleAvailable);
        }

        [Fact]
        public async Task GetNearbyPagedAsync_ShouldReturnStationsWithinRadius()
        {
            _repoMock.Setup(r => r.Query()).Returns(GetSampleStationsList().AsQueryableForTest());

            double lat = 21.0;
            double lng = 105.0;
            double radiusKm = 2000;
            int page = 1;
            int pageSize = 10;

            var pagedResult = await _service.GetNearbyPagedAsync(lat, lng, radiusKm, page, pageSize);

            Assert.NotEmpty(pagedResult.Items);
            Assert.Contains(pagedResult.Items, s => s.Name == "Station A");
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnFilteredResults()
        {
            _repoMock.Setup(r => r.Query()).Returns(GetSampleStationsList().AsQueryableForTest());

            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, searchQuery: "station b");

            Assert.Single(result.Items);
            Assert.Equal("Station B", result.Items.First().Name);
        }

        [Fact]
        public async Task GetPagedAsync_WithFilter_ShouldReturnFilteredResults()
        {
            _repoMock.Setup(r => r.Query()).Returns(GetSampleStationsList().AsQueryableForTest());

            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, filterField: "isActive", filterValue: "true");

            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, s => Assert.True(s.IsActive));
        }
    }
}
