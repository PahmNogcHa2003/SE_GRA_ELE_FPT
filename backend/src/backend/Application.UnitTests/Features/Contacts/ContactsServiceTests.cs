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
using System.Linq;
using System.Threading.Tasks;

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

        private async Task SeedAsync()
        {
            var c1 = new Contact("Title 1", "Content 1", DateTimeOffset.UtcNow)
            { Id = 1, Email = "a@test.com" };

            var c2 = new Contact("Title 2", "Content 2", DateTimeOffset.UtcNow)
            { Id = 2, Email = "b@test.com" };

            var c3 = new Contact("Title 3", "Content 3", DateTimeOffset.UtcNow)
            { Id = 3, Email = "c@test.com" };

            _context.Contacts.AddRange(c1, c2, c3);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertContact()
        {
            var dto = new CreateContactDTO
            {
                Email = "demo@test.com",
                PhoneNumber = "012345",
                Title = "New contact",
                Content = "Content here"
            };

            var created = await _service.CreateAsync(dto);

            created.Should().NotBeNull();
            created.Title.Should().Be("New contact");

            var db = await _repo.GetByIdAsync(created.);
            db.Should().NotBeNull();
            db!.Status.Should().Be(Contact.StatusOpen);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPaged()
        {
            await SeedAsync();

            var result = await _service.GetPagedAsync(1, 2);

            result.Items.Count().Should().Be(2);
            result.TotalCount.Should().Be(3);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldSortById()
        {
            await SeedAsync();

            var result = await _service.GetPagedAsync(1, 10);

            result.Items.First().Id.Should().Be(1);
            result.Items.Last().Id.Should().Be(3);
        }

        [Fact]
        public async Task SubmitReply_ShouldUpdateStatusToReplied()
        {
            await SeedAsync();

            var contact = await _repo.GetByIdAsync(1);
            contact!.SubmitReply(99, "Reply message", DateTimeOffset.UtcNow);

            _repo.Update(contact);
            await _uow.SaveChangesAsync();

            var updated = await _repo.GetByIdAsync(1);
            updated!.Status.Should().Be(Contact.StatusReplied);
            updated.ReplyContent.Should().Be("Reply message");
            updated.ReplyById.Should().Be(99);
        }

        [Fact]
        public async Task SubmitReply_EmptyContent_ShouldThrow()
        {
            await SeedAsync();

            var contact = await _repo.GetByIdAsync(1);

            Action act = () => contact!.SubmitReply(1, "", DateTimeOffset.UtcNow);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task MarkAsSent_ShouldSetIsReplySent()
        {
            await SeedAsync();

            var contact = await _repo.GetByIdAsync(1);
            contact!.SubmitReply(10, "OK", DateTimeOffset.UtcNow);

            contact.MarkAsSent();

            _repo.Update(contact);
            await _uow.SaveChangesAsync();

            var updated = await _repo.GetByIdAsync(1);
            updated!.IsReplySent.Should().BeTrue();
        }

        [Fact]
        public async Task MarkAsSent_WhenNotReplied_ShouldThrow()
        {
            await SeedAsync();

            var c = await _repo.GetByIdAsync(1);

            Action act = () => c!.MarkAsSent();

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task Close_ShouldUpdateStatusClosed()
        {
            await SeedAsync();

            var contact = await _repo.GetByIdAsync(1);
            contact!.Close(DateTimeOffset.UtcNow);

            _repo.Update(contact);
            await _uow.SaveChangesAsync();

            var updated = await _repo.GetByIdAsync(1);
            updated!.Status.Should().Be(Contact.StatusClosed);
            updated.ClosedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task ReplyToClosedContact_ShouldThrow()
        {
            await SeedAsync();

            var contact = await _repo.GetByIdAsync(1);
            contact!.Close(DateTimeOffset.UtcNow);

            Action act = () =>
                contact.SubmitReply(1, "Cannot reply", DateTimeOffset.UtcNow);

            act.Should().Throw<InvalidOperationException>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
