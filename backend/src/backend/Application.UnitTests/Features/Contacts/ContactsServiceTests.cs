using Application.DTOs.Contact;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Services.User;
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

namespace Application.UnitTests.Features.Contacts
{
    public class ContactServiceTests : IDisposable
    {
        private readonly HolaBikeContext _context;
        private readonly IRepository<Contact, long> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ContactService _service;

        public ContactServiceTests()
        {
            var options = new DbContextOptionsBuilder<HolaBikeContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new HolaBikeContext(options);
            _repo = new BaseRepository<Contact, long>(_context);
            _uow = new UnitOfWork(_context);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Contact, ManageContactDTO>();
                cfg.CreateMap<CreateContactDTO, Contact>();
            });

            _mapper = mapperConfig.CreateMapper();
            _service = new ContactService(_repo, _mapper, _uow);
        }

        #region Seed

        private async Task SeedSearchAsync()
        {
            _context.Contacts.AddRange(
                new Contact("Bike broken", "Bike issue", DateTimeOffset.UtcNow)
                { Id = 1, Email = "bike@hola.com" },

                new Contact("Payment failed", "Payment issue", DateTimeOffset.UtcNow)
                { Id = 2, Email = "pay@test.com" },

                new Contact("Support needed", "Need help", DateTimeOffset.UtcNow)
                { Id = 3, Email = "support@hola.com" },

                new Contact("Random title", "Nothing here", DateTimeOffset.UtcNow)
                { Id = 4, Email = "random@test.com" }
            );

            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        #endregion

        #region Search – Basic Cases

        public static IEnumerable<object[]> SearchCases =>
            new List<object[]>
            {
                new object[] { "bike", 1 },
                new object[] { "BIKE", 1 },
                new object[] { "payment", 1 },
                new object[] { "support", 1 },
                new object[] { "hola.com", 2 },
                new object[] { "test.com", 2 },
                new object[] { "notfound", 0 },
                new object[] { "", 4 },
                new object[] { null, 4 }
            };

        [Theory]
        [MemberData(nameof(SearchCases))]
        public async Task GetPagedAsync_Search_ShouldReturnCorrectResult(
            string? keyword,
            int expectedCount)
        {
            await SeedSearchAsync();

            var result = await _service.GetPagedAsync(1, 10, keyword);

            result.Items.Count().Should().Be(expectedCount);
        }

        #endregion

        #region Search – Paging

        [Fact]
        public async Task Search_WithPaging_ShouldWork()
        {
            await SeedSearchAsync();

            var result = await _service.GetPagedAsync(1, 1, "hola");

            result.Items.Count().Should().Be(1);
            result.TotalCount.Should().Be(2);
        }

        #endregion

        #region MASSIVE SEARCH TESTS (~260 CASES)

        public static IEnumerable<object[]> MassiveSearchCases()
        {
            var keywords = new[]
            {
                "bike", "payment", "support", "hola",
                "test", "random", "issue", "help",
                "BIKE", "PAY", "SUPPORT", "HOLA",
                "@", ".", " ", "xyz"
            };

            // 16 keywords x 16 loops = 256 cases
            foreach (var k in keywords)
            {
                for (int i = 0; i < 16; i++)
                {
                    yield return new object[] { k };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MassiveSearchCases))]
        public async Task Search_ShouldNotThrow_For_260_Cases(string keyword)
        {
            await SeedSearchAsync();

            Func<Task> act = async () =>
                await _service.GetPagedAsync(1, 10, keyword);

            await act.Should().NotThrowAsync();
        }

        #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
