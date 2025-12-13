using Application.DTOs.Tickets;
using Application.Services.Staff;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Features.TicketPlanPrices
{
    public class TicketPlanPriceServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly BaseRepository<TicketPlanPrice, long> _repo;
        private readonly UnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly TicketPlanPriceService _service;

        public TicketPlanPriceServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);
            _repo = new BaseRepository<TicketPlanPrice, long>(_context);
            _uow = new UnitOfWork(_context);

            var cfg = new MapperConfiguration(c =>
            {
                c.CreateMap<TicketPlanPrice, TicketPlanPriceDTO>();
                c.CreateMap<TicketPlanPriceDTO, TicketPlanPrice>();
            });

            _mapper = cfg.CreateMapper();

            _service = new TicketPlanPriceService(_repo, _mapper, _uow);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreate()
        {
            var dto = new TicketPlanPriceDTO
            {
                PlanId = 1,
                VehicleType = "bike",
                Price = 5000
            };

            var created = await _service.CreateAsync(dto);

            created.Should().NotBeNull();
            created.Price.Should().Be(5000);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
