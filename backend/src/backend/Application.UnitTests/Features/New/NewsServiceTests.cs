using Application.DTOs.New;
using Application.DTOs.TagNew;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.Staff.Repository;
using Application.Photo;
using Application.Services.Staff;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Application.UnitTests.Features.New
{
    public class NewsServiceTests
    {
        private readonly Mock<INewsRepository> _newsRepo;
        private readonly Mock<IPhotoService> _photoService;
        private readonly Mock<IRepository<Tag, long>> _tagRepo;
        private readonly Mock<IRepository<TagNew, long>> _tagNewRepo;
        private readonly Mock<IUnitOfWork> _uow;
        private readonly Mock<IHttpContextAccessor> _httpContext;
        private readonly Mock<ILogger<NewsService>> _logger;
        private readonly IMapper _mapper;
        private readonly NewsService _service;

        public NewsServiceTests()
        {
            _newsRepo = new Mock<INewsRepository>();
            _photoService = new Mock<IPhotoService>();
            _tagRepo = new Mock<IRepository<Tag, long>>();
            _tagNewRepo = new Mock<IRepository<TagNew, long>>();
            _uow = new Mock<IUnitOfWork>();
            _httpContext = new Mock<IHttpContextAccessor>();
            _logger = new Mock<ILogger<NewsService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NewsDTO, News>().ReverseMap();
                cfg.CreateMap<TagNewDTO, TagNew>().ReverseMap();
            });

            _mapper = config.CreateMapper();

            var user = new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.NameIdentifier, "1") })
            );

            var httpContext = new DefaultHttpContext { User = user };
            _httpContext.Setup(x => x.HttpContext).Returns(httpContext);

            _service = new NewsService(
                _newsRepo.Object,
                _photoService.Object,
                _tagRepo.Object,
                _tagNewRepo.Object,
                _mapper,
                _uow.Object,
                _httpContext.Object,
                _logger.Object
            );
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNews()
        {
            var dto = new NewsDTO
            {
                Title = "Tin mới",
                Slug = "tin-moi",
                Content = "Nội dung",
                TagIds = new List<long> { 1, 2 }
            };

            _newsRepo.Setup(r => r.AddAsync(It.IsAny<News>(), default))
                .Callback<News, CancellationToken>((e, _) => e.Id = 1);

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Tin mới", result.Title);
            Assert.Equal(1, result.Id);
            _newsRepo.Verify(r => r.AddAsync(It.IsAny<News>(), default), Times.Once);
            _tagNewRepo.Verify(r => r.AddAsync(It.IsAny<TagNew>(), default), Times.Exactly(2));
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateNews()
        {
            var news = new News { Id = 1, Title = "Cũ" };

            _newsRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(news);

            _tagNewRepo.Setup(r => r.Query()).Returns(new List<TagNew>
            {
                new TagNew(1, 10)
            }.AsQueryable());

            var dto = new NewsDTO
            {
                Title = "Mới",
                TagIds = new List<long> { 2, 3 }
            };

            var result = await _service.UpdateAsync(1, dto);

            Assert.NotNull(result);
            Assert.Equal("Mới", result.Title);
            _tagNewRepo.Verify(r => r.Remove(It.IsAny<TagNew>()), Times.Once);
            _tagNewRepo.Verify(r => r.AddAsync(It.IsAny<TagNew>(), default), Times.Exactly(2));
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteNews()
        {
            var news = new News { Id = 1 };

            _newsRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(news);
            _tagNewRepo.Setup(r => r.Query()).Returns(new List<TagNew>
            {
                new TagNew(1, 5)
            }.AsQueryable());

            var result = await _service.DeleteAsync(1);

            Assert.NotNull(result);
            _tagNewRepo.Verify(r => r.Remove(It.IsAny<TagNew>()), Times.Once);
            _newsRepo.Verify(r => r.Remove(news), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateBannerAsync_ShouldUpdateBanner()
        {
            var news = new News { Id = 1, BannerPublicId = "old-id" };

            _newsRepo.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(news);

            var uploadResult = new PhotoUploadResult
            {
                Url = "https://new-image",
                PublicId = "new-id"
            };

            _photoService.Setup(p => p.AddPhotoAsync(It.IsAny<IFormFile>(), PhotoPreset.NewsBanner))
                .ReturnsAsync(uploadResult);

            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(1024);

            var result = await _service.UpdateBannerAsync(1, file.Object);

            Assert.NotNull(result);
            Assert.Equal("https://new-image", result.Banner);
            _photoService.Verify(p => p.DeletePhotoAsync("old-id"), Times.Once);
            _uow.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }
    }
}
