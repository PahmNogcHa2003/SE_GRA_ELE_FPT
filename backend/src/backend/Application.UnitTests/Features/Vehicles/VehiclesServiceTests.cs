using Application.DTOs.Vehicle;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Services.Staff;
using AutoMapper;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Application.UnitTests.Features.Vehicles
{
    public class VehiclesServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IRepository<Vehicle, long> _vehicleRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        private readonly VehiclesService _service;

        public VehiclesServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);

            _vehicleRepo = new BaseRepository<Vehicle, long>(_context);
            _uow = new UnitOfWork(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Vehicle, VehicleDTO>();
            });
            _mapper = mapperConfig.CreateMapper();

            _service = new VehiclesService(_vehicleRepo, _mapper, _uow);
        }

        // ---------------------------------------------------------------------
        // Seed Data
        // ---------------------------------------------------------------------

        private async Task SeedVehiclesAsync()
        {
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Id = 1,
                    BikeCode = "A-001",
                    Status = "available",
                    BatteryLevel = 80,
                    ChargingStatus = false
                },
                new Vehicle
                {
                    Id = 2,
                    BikeCode = "A-002",
                    Status = "unavailable",
                    BatteryLevel = 40,
                    ChargingStatus = true
                },
                new Vehicle
                {
                    Id = 3,
                    BikeCode = "B-123",
                    Status = "maintenance",
                    BatteryLevel = 60,
                    ChargingStatus = false
                }
            };

            _context.Vehicles.AddRange(vehicles);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnFilteredByBikeCode()
        {
            await SeedVehiclesAsync();

            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, searchQuery: "A-00");

            result.Items.Count().Should().Be(2);
            result.Items.Select(v => v.BikeCode).Should().Contain("A-001");
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnFilteredByBatteryLevel()
        {
            await SeedVehiclesAsync();

            var result = await _service.GetPagedAsync(page: 1, pageSize: 10, searchQuery: "80");

            result.Items.Should().ContainSingle();
            result.Items.First().BikeCode.Should().Be("A-001");
        }

        [Fact]
        public async Task GetPagedAsync_FilterByStatus_ShouldReturnCorrectResults()
        {
            await SeedVehiclesAsync();

            var result = await _service.GetPagedAsync(
                page: 1,
                pageSize: 10,
                filterField: "status",
                filterValue: "available"
            );

            result.Items.Should().ContainSingle();
            result.Items.First().BikeCode.Should().Be("A-001");
        }

        [Fact]
        public async Task GetPagedAsync_FilterByChargingStatus_ShouldReturnCorrectResults()
        {
            await SeedVehiclesAsync();

            var result = await _service.GetPagedAsync(
                page: 1,
                pageSize: 10,
                filterField: "chargingStatus",
                filterValue: "true"
            );

            result.Items.Should().ContainSingle();
            result.Items.First().BikeCode.Should().Be("A-002");
        }

        [Fact]
        public async Task GetPagedAsync_DefaultSort_ShouldReturnOrderedById()
        {
            await SeedVehiclesAsync();

            var result = await _service.GetPagedAsync(page: 1, pageSize: 10);

            result.Items.First().Id.Should().Be(1);
            result.Items.Last().Id.Should().Be(3);
        }

        // ---------------------------------------------------------------------
        // Cleanup
        // ---------------------------------------------------------------------

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
