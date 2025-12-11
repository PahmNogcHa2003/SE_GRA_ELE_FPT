using Application.DTOs.UserProfile;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Repository;
using Application.Photo;
using Application.Services.User;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTests.Features.UserProfiles
{
    public class UserProfilesServiceTests
    {
        private readonly Mock<IRepository<UserProfile, long>> _repo = new();
        private readonly Mock<IUserProfilesRepository> _userProfilesRepository = new();
        private readonly Mock<IPhotoService> _photoService = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private readonly IMapper _mapper;

        public UserProfilesServiceTests()
        {
            var cfg = new MapperConfiguration(x =>
            {
                x.CreateMap<UserProfile, UserProfileDTO>();
            });
            _mapper = cfg.CreateMapper();
        }

        private UserProfilesService CreateService()
        {
            return new UserProfilesService(
                _repo.Object,
                _mapper,
                _uow.Object,
                _userProfilesRepository.Object,
                _photoService.Object
            );
        }

        [Fact]
        public async Task UpdateBasicByUserId_Success()
        {
            var svc = CreateService();
            long userId = 1;

            var userProfile = new UserProfile
            {
                Id = 10,
                UserId = userId,
                FullName = "Old Name",
                Gender = "Male",
                AddressDetail = "Old Address",
                User = new Domain.Entities.AspNetUser { PhoneNumber = "000" }
            };

            var list = new List<UserProfile> { userProfile };
            _repo.Setup(x => x.Query())
                .Returns(list.AsQueryable().BuildMock());

            _repo.Setup(x => x.Update(It.IsAny<UserProfile>()));
            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _userProfilesRepository.Setup(x => x.GetUserProfileWithVerify(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserProfileDTO { UserId = userId, FullName = "New Name" });

            var dto = new UpdateUserProfileBasicDTO
            {
                FullName = "New Name",
                Gender = "Female",
                PhoneNumber = "123456"
            };

            var result = await svc.UpdateBasicByUserIdAsync(userId, dto, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("New Name", result.FullName);
            Assert.Equal("Female", userProfile.Gender);
            Assert.Equal("123456", userProfile.User.PhoneNumber);
        }

        [Fact]
        public async Task UpdateBasicByUserId_ReturnsNull_WhenUserNotFound()
        {
            var svc = CreateService();

            _repo.Setup(x => x.Query())
                .Returns(new List<UserProfile>().AsQueryable().BuildMock());

            var result = await svc.UpdateBasicByUserIdAsync(1,
                new UpdateUserProfileBasicDTO(),
                CancellationToken.None);

            Assert.Null(result);
        }
    
        [Fact]
        public async Task UpdateAvatar_Success()
        {
            var svc = CreateService();
            long userId = 1;

            var entity = new UserProfile
            {
                Id = 10,
                UserId = userId,
                AvatarPublicId = null
            };

            _repo.Setup(x => x.Query())
                .Returns(new List<UserProfile> { entity }.AsQueryable().BuildMock());

            // mock file upload
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.Length).Returns(100);

            _photoService.Setup(x => x.AddPhotoAsync(fileMock.Object, PhotoPreset.Avatar))
                .ReturnsAsync(new PhotoUploadResult
                {
                    Url = "http://cdn/avatar.jpg",
                    PublicId = "123"
                });

            _repo.Setup(x => x.Update(It.IsAny<UserProfile>()));
            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await svc.UpdateAvatarAsync(userId, fileMock.Object, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("http://cdn/avatar.jpg", entity.AvatarUrl);
            Assert.Equal("123", entity.AvatarPublicId);
        }

        [Fact]
        public async Task UpdateAvatar_ReturnsNull_WhenUserNotFound()
        {
            var svc = CreateService();
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.Length).Returns(100);

            _repo.Setup(x => x.Query())
                .Returns(new List<UserProfile>().AsQueryable().BuildMock());

            var result = await svc.UpdateAvatarAsync(1, fileMock.Object, CancellationToken.None);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAvatar_Throws_WhenFileInvalid()
        {
            var svc = CreateService();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                svc.UpdateAvatarAsync(1, null, CancellationToken.None)
            );

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.Length).Returns(0);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                svc.UpdateAvatarAsync(1, fileMock.Object, CancellationToken.None)
            );
        }

        [Fact]
        public async Task UpdateAvatar_DeletesOldPhoto_IfExists()
        {
            var svc = CreateService();
            long userId = 1;

            var entity = new UserProfile
            {
                Id = 10,
                UserId = userId,
                AvatarPublicId = "old_img"
            };

            _repo.Setup(x => x.Query())
                .Returns(new List<UserProfile> { entity }.AsQueryable().BuildMock());

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.Length).Returns(100);

            // mock delete
            _photoService.Setup(x => x.DeletePhotoAsync("old_img"))
                .Returns((Task<string>)Task.CompletedTask)
                .Verifiable();

            // mock upload
            _photoService.Setup(x => x.AddPhotoAsync(fileMock.Object, PhotoPreset.Avatar))
                .ReturnsAsync(new PhotoUploadResult
                {
                    Url = "new-url",
                    PublicId = "new-id"
                });

            _repo.Setup(x => x.Update(It.IsAny<UserProfile>()));
            _uow.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await svc.UpdateAvatarAsync(userId, fileMock.Object, CancellationToken.None);

            _photoService.Verify(x => x.DeletePhotoAsync("old_img"), Times.Once);
            Assert.Equal("new-url", entity.AvatarUrl);
            Assert.Equal("new-id", entity.AvatarPublicId);
        }
    }
}
