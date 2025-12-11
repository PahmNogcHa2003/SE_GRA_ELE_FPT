using Application.DTOs.Tag;
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

namespace Application.UnitTests.Features.Tags
{
    public class TagsServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IRepository<Tag, long> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly TagService _service;

        public TagsServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);
            _repo = new BaseRepository<Tag, long>(_context);
            _uow = new UnitOfWork(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tag, TagDTO>();
            });

            _mapper = mapperConfig.CreateMapper();
            _service = new TagService(_repo, _mapper, _uow);
        }

        private async Task SeedAsync()
        {
            var items = new List<Tag>
            {
                new Tag { Id = 1, Name = "Công nghệ" },
                new Tag { Id = 2, Name = "Xe điện" },
                new Tag { Id = 3, Name = "Thời tiết" }
            };

            _context.Tags.AddRange(items);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnFilteredByName()
        {
            await SeedAsync();

            var result = await _service.GetPagedAsync(1, 10, searchQuery: "điện");

            result.Items.Should().ContainSingle();
            result.Items.First().Name.Should().Be("Xe điện");
        }

        [Fact]
        public async Task GetPagedAsync_WithSearch_ShouldReturnMultiple()
        {
            await SeedAsync();

            var result = await _service.GetPagedAsync(1, 10, searchQuery: "ông");

            result.Items.Count().Should().Be(1);
            result.Items.First().Name.Should().Be("Công nghệ");
        }

        [Fact]
        public async Task GetPagedAsync_DefaultSort_ShouldReturnOrderedById()
        {
            await SeedAsync();

            var result = await _service.GetPagedAsync(1, 10);

            result.Items.First().Id.Should().Be(1);
            result.Items.Last().Id.Should().Be(3);
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertTag()
        {
            var tag = new Tag { Name = "Tin mới" };

            await _repo.AddAsync(tag);
            await _uow.SaveChangesAsync();

            var saved = await _repo.GetByIdAsync(tag.Id);

            saved.Should().NotBeNull();
            saved!.Name.Should().Be("Tin mới");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
