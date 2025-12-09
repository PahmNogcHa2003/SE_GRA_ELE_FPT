using Application.DTOs.CategoriesVehicle;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Services.Staff;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Features.CategoriesVehicles
{
    public class CategoriesVehicleServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IRepository<CategoriesVehicle, long> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly CategoriesVehicleService _service;

        public CategoriesVehicleServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);
            _repo = new BaseRepository<CategoriesVehicle, long>(_context);
            _uow = new UnitOfWork(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoriesVehicle, CategoriesVehicleDTO>();
            });
            _mapper = mapperConfig.CreateMapper();

            _service = new CategoriesVehicleService(_repo, _mapper, _uow);
        }

        private async Task SeedCategoriesAsync()
        {
            var categories = new List<CategoriesVehicle>
            {
                new CategoriesVehicle { Id = 1, Name = "Xe Điện", Description = "Loại xe chạy điện", IsActive = true },
                new CategoriesVehicle { Id = 2, Name = "Xe Đạp", Description = "Xe đạp thường", IsActive = true },
                new CategoriesVehicle { Id = 3, Name = "Xe Hư", Description = "Đang bảo trì", IsActive = false }
            };

            _context.CategoriesVehicles.AddRange(categories);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldFilterByName()
        {
            await SeedCategoriesAsync();

            var result = await _service.GetPagedAsync(1, 10, searchQuery: "điện");

            result.Items.Should().ContainSingle();
            result.Items.First().Name.Should().Be("Xe Điện");
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldFilterByDescription()
        {
            await SeedCategoriesAsync();

            var result = await _service.GetPagedAsync(1, 10, searchQuery: "bảo trì");

            result.Items.Should().ContainSingle();
            result.Items.First().Name.Should().Be("Xe Hư");
        }

        [Fact]
        public async Task GetPagedAsync_FilterByIsActive_ShouldReturnActiveItems()
        {
            await SeedCategoriesAsync();

            var result = await _service.GetPagedAsync(1, 10, "isActive", "true");

            result.Items.Count().Should().Be(2);
        }

        [Fact]
        public async Task GetPagedAsync_FilterByIsActive_ShouldReturnInactiveItems()
        {
            await SeedCategoriesAsync();

            var result = await _service.GetPagedAsync(1, 10, "isActive", "false");

            result.Items.Should().ContainSingle();
            result.Items.First().Name.Should().Be("Xe Hư");
        }

        [Fact]
        public async Task GetPagedAsync_DefaultSort_ShouldBeSortedById()
        {
            await SeedCategoriesAsync();

            var result = await _service.GetPagedAsync(1, 10);

            result.Items.First().Id.Should().Be(1);
            result.Items.Last().Id.Should().Be(3);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
