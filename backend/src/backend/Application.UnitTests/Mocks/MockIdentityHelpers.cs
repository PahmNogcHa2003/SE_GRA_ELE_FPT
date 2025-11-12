using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
// Thêm 2 using này nếu bạn dùng .NET Core 3.1 trở lên (rất nên có)
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;


namespace Application.UnitTests.Mocks
{
    public static class MockIdentityHelpers
    {
        // Hàm này tạo ra một Mock<UserManager<AspNetUser>>
        public static Mock<UserManager<AspNetUser>> GetMockUserManager()
        {
            // UserManager yêu cầu rất nhiều tham số, ta mock cái quan trọng nhất
            // là IUserStore và truyền các tham số khác là null hoặc mock rỗng.
            var mockStore = new Mock<IUserStore<AspNetUser>>();

            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions();
            mockOptions.Setup(o => o.Value).Returns(identityOptions);

            var mockPasswordHasher = new Mock<IPasswordHasher<AspNetUser>>();
            var mockUserValidators = new IUserValidator<AspNetUser>[0];
            var mockPassValidators = new IPasswordValidator<AspNetUser>[0];
            var mockNormalizer = new Mock<ILookupNormalizer>();
            var mockErrorDescriber = new Mock<IdentityErrorDescriber>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockLogger = new Mock<ILogger<UserManager<AspNetUser>>>();

            var mockUserManager = new Mock<UserManager<AspNetUser>>(
                mockStore.Object,
                mockOptions.Object,
                mockPasswordHasher.Object,
                mockUserValidators,
                mockPassValidators,
                mockNormalizer.Object,
                mockErrorDescriber.Object,
                mockServiceProvider.Object,
                mockLogger.Object
            );

            return mockUserManager;
        }

        // =================================================================
        // ⭐ ĐÃ SỬA Ở ĐÂY
        // =================================================================
        // Hàm này tạo ra một Mock<SignInManager<AspNetUser>>
        // Giờ nó sẽ NHẬN UserManager mock từ bên ngoài
        public static Mock<SignInManager<AspNetUser>> GetMockSignInManager(Mock<UserManager<AspNetUser>> mockUserManager)
        {
            // Xoá dòng: var mockUserManager = GetMockUserManager();

            // SignInManager cũng yêu cầu nhiều dependency, ta mock rỗng
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<AspNetUser>>();
            var mockOptions = new Mock<IOptions<IdentityOptions>>();
            var mockLogger = new Mock<ILogger<SignInManager<AspNetUser>>>();
            var mockAuthScheme = new Mock<IAuthenticationSchemeProvider>();
            var mockUserConfirmation = new Mock<IUserConfirmation<AspNetUser>>();

            var mockSignInManager = new Mock<SignInManager<AspNetUser>>(
                mockUserManager.Object, // <-- Dùng mock được truyền vào
                mockContextAccessor.Object,
                mockClaimsFactory.Object,
                mockOptions.Object,
                mockLogger.Object,
                mockAuthScheme.Object,
                mockUserConfirmation.Object
            );

            return mockSignInManager;
        }
    }
}