using Application.DTOs.Auth;
using Application.DTOs.UserProfile;
using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Identity;
using Application.Interfaces.User.Service;
using Application.Services.Identity;
using Application.UnitTests.Mocks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Features.Identity
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<AspNetUser>> _mockUserManager;
        private readonly Mock<SignInManager<AspNetUser>> _mockSignInManager;
        private readonly Mock<IJwtTokenService> _mockTokenService;
        private readonly Mock<IUserProfilesService> _mockProfileService;
        private readonly Mock<IUserDevicesService> _mockUserDevicesService;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly Mock<IUserWalletService> _mockUserWalletService;
        private readonly Mock<IEmailRepository> _mockEmailService;

        private readonly IAuthService _service;

        public AuthServiceTests()
        {
            _mockUow = MockUnitOfWork.GetMockUnitOfWork();
            _mockTokenService = new Mock<IJwtTokenService>();
            _mockProfileService = new Mock<IUserProfilesService>();
            _mockUserDevicesService = new Mock<IUserDevicesService>();
            _mockWalletService = new Mock<IWalletService>();
            _mockUserWalletService = new Mock<IUserWalletService>();
            _mockEmailService = new Mock<IEmailRepository>();

            var mockUserStore = new Mock<IUserStore<AspNetUser>>();
            _mockUserManager = new Mock<UserManager<AspNetUser>>(
                mockUserStore.Object,
                null, null, null, null, null, null, null, null
            );

            _mockSignInManager = new Mock<SignInManager<AspNetUser>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<AspNetUser>>(),
                null, null, null, null
            );

            _service = new AuthService(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockTokenService.Object,
                _mockProfileService.Object,
                _mockUserDevicesService.Object,
                _mockWalletService.Object,
                _mockUow.Object,
                _mockUserWalletService.Object,
                _mockEmailService.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_WhenEmailAlreadyExists()
        {
            var registerDto = new RegisterDTO { Email = "test@example.com", Password = "Pass!", ConfirmPassword = "Pass!" };
            var existingUser = new AspNetUser { Email = "test@example.com" };

            _mockUserManager.Setup(u => u.FindByEmailAsync(registerDto.Email))
                            .ReturnsAsync(existingUser);

            var result = await _service.RegisterAsync(registerDto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Email already exists.", result.Message);
            _mockUow.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSucceed_WhenDataIsValid()
        {
            var registerDto = new RegisterDTO
            {
                Email = "new@example.com",
                Password = "NewPassword123!",
                ConfirmPassword = "NewPassword123!",
                FullName = "Test User"
            };

            _mockUserManager.Setup(u => u.FindByEmailAsync(registerDto.Email))
                            .ReturnsAsync((AspNetUser)null);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<AspNetUser>(), registerDto.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<AspNetUser>(), "User"))
                            .ReturnsAsync(IdentityResult.Success);
            _mockProfileService.Setup(p => p.CreateAsync(It.IsAny<UserProfileDTO>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new UserProfileDTO());
            _mockWalletService.Setup(w => w.CreateAsync(It.IsAny<WalletDTO>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(new WalletDTO());

            var result = await _service.RegisterAsync(registerDto);

            Assert.True(result.IsSuccess);
            Assert.Equal("Registration successful.", result.Message);

            _mockUserManager.Verify(u => u.CreateAsync(It.IsAny<AspNetUser>(), registerDto.Password), Times.Once);
            _mockUserManager.Verify(u => u.AddToRoleAsync(It.IsAny<AspNetUser>(), "User"), Times.Once);
            _mockProfileService.Verify(p => p.CreateAsync(It.IsAny<UserProfileDTO>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockWalletService.Verify(w => w.CreateAsync(It.IsAny<WalletDTO>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldRollback_WhenCreateProfileFails()
        {
            var registerDto = new RegisterDTO { Email = "fail@example.com", Password = "Pass!", ConfirmPassword = "Pass!" };

            _mockUserManager.Setup(u => u.FindByEmailAsync(registerDto.Email))
                            .ReturnsAsync((AspNetUser)null);
            _mockUserManager.Setup(u => u.CreateAsync(It.IsAny<AspNetUser>(), registerDto.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(u => u.AddToRoleAsync(It.IsAny<AspNetUser>(), "User"))
                            .ReturnsAsync(IdentityResult.Success);
            _mockProfileService.Setup(p => p.CreateAsync(It.IsAny<UserProfileDTO>(), It.IsAny<CancellationToken>()))
                               .ThrowsAsync(new Exception("Profile error"));

            var result = await _service.RegisterAsync(registerDto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Registration failed due to an internal server error.", result.Message);

            _mockUow.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.CommitTransactionAsync(It.IsAny<IDbContextTransaction>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(u => u.RollbackTransactionAsync(It.IsAny<IDbContextTransaction>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginDTO { Email = "user@example.com", Password = "Password123!" };
            var fakeUser = new AspNetUser { Id = 1, Email = loginDto.Email };
            var fakeRoles = new List<string> { "User" };

            _mockUserManager.Setup(u => u.FindByEmailAsync(loginDto.Email))
                            .ReturnsAsync(fakeUser);
            _mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(fakeUser, loginDto.Password, false))
                              .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(t => t.GenerateJwtTokenAsync(fakeUser))
                             .ReturnsAsync("mock.jwt.token.string");
            _mockUserManager.Setup(u => u.GetRolesAsync(fakeUser))
                            .ReturnsAsync(fakeRoles);
            _mockProfileService.Setup(p => p.GetByUserIdAsync(fakeUser.Id, It.IsAny<CancellationToken>()))
                               .ReturnsAsync(new UserProfileDTO { FullName = "Test User" });

            var result = await _service.LoginAsync(loginDto);

            Assert.True(result.IsSuccess);
            Assert.Equal("mock.jwt.token.string", result.Token);
            Assert.Contains("User", result.Roles);
        }
    }
}
