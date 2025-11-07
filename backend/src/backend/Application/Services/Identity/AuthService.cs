using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.UserDevice;
using Application.DTOs.UserProfile;
using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Interfaces.Identity;
using Application.Interfaces.User.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Application.Services.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly IJwtTokenService _tokenService;
        private readonly IUserProfilesService _profileService;
        private readonly IUserDevicesService _userDevicesService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletService _walletService;
        private readonly IUserWalletService _userWalletService;

        public AuthService(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            IJwtTokenService tokenService,
            IUserProfilesService userProfilesService,
            IUserDevicesService userDevicesService,
            IWalletService walletService,
            IUnitOfWork unitOfWork,
            IUserWalletService userWalletService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _profileService = userProfilesService;
            _userDevicesService = userDevicesService;
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _userWalletService = userWalletService;
        }

        // --- REGISTER ---
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model, CancellationToken ct = default)
        {
            // 1) Validate đơn giản (nên validate ở DTO/FluentValidation)
            if (model.Password != model.ConfirmPassword)
            {
                return new AuthResponseDTO { IsSuccess = false, Message = "Password and Confirm Password do not match." };
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new AuthResponseDTO { IsSuccess = false, Message = "Email already exists." };
            }

            var now = DateTimeOffset.UtcNow;
            var user = new AspNetUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber?.ToString(),
                CreatedDate = now
            };

            // 2) Transaction (đúng cách)
            await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // 3) Tạo user
                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    var msg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    await _unitOfWork.RollbackTransactionAsync(tx, ct);
                    return new AuthResponseDTO { IsSuccess = false, Message = msg };
                }

                // 4) Gán role (không có ct trong API Identity)
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    var msg = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    await _unitOfWork.RollbackTransactionAsync(tx, ct);
                    return new AuthResponseDTO { IsSuccess = false, Message = msg };
                }

                // 5) Tạo hồ sơ người dùng (Application DbContext)
                var userProfileDto = new UserProfileDTO
                {
                    UserId = user.Id,
                    Dob = model.DateOfBirth,
                    EmergencyName = model.EmergencyName,
                    EmergencyPhone = model.EmergencyPhone,
                    NumberCard = model.IdentityNumber,
                    Gender = model.Gender,
                    FullName = model.FullName,
                    AddressDetail = model.Address,
                    ProvinceCode = model.ProvinceId,
                    ProvinceName = model.ProvinceName,
                    WardCode = model.WardId,
                    WardName = model.WardName,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                await _profileService.CreateAsync(userProfileDto, ct);

                var wallet = new WalletDTO
                {
                    UserId = user.Id,
                    Balance = 0m,
                    TotalDebt = 0m,
                    Status = "Active",
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _walletService.CreateAsync(wallet, ct);
                // 6) Lưu & commit
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(tx, ct);

                return new AuthResponseDTO { IsSuccess = true, Message = "Registration successful." };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync(tx, ct);
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Registration failed due to an internal server error."
                };
            }
        }

        // --- LOGIN ---
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials." };

            var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials." };

            // Lưu thông tin thiết bị (nếu có)
            if (model.DeviceId != null)
            {
                var userDeviceDto = new CreateUserDeviceDTO
                {
                    UserId = user.Id,
                    DeviceId = model.DeviceId, 
                    PushToken = model.PushToken,
                    Platform = model.Platform
                };

                await _userDevicesService.HandleDeviceLoginAsync(userDeviceDto, ct);
            }


            // Tạo JWT
            var jwt = await _tokenService.GenerateJwtTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var profile = await _profileService.GetByUserIdAsync(user.Id);
            return new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = jwt,
                Roles = [.. roles]

            };
        }
        private long? GetUserIdAsLong(ClaimsPrincipal userPrincipal)
        {
            var idStr = _userManager.GetUserId(userPrincipal);
            if (long.TryParse(idStr, out var id))
                return id;
            return null;
        }

        public async Task<MeDTO?> GetMeAsync(ClaimsPrincipal userPrincipal, CancellationToken ct = default)
        {
            // Lấy user hiện tại từ Claims
            var userId = GetUserIdAsLong(userPrincipal);
            if (userId == null) return null;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null) return null;

            // Lấy roles
            var roles = await _userManager.GetRolesAsync(user);

            // Lấy hồ sơ (nếu có)
            var profile = await _profileService.GetByUserIdAsync(user.Id, ct);
            // Giả định bạn đã có method này trong IUserProfilesService
            // Nếu chưa có, bạn có thể thêm một method tương tự hoặc dùng Query service hiện có.
            var wallet = await _userWalletService.GetByUserIdAsync(user.Id, ct);
            var me = new MeDTO
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = profile?.FullName,
                AvatarUrl = profile?.AvatarUrl,
                CreatedDate = user.CreatedDate,
                Dob = profile?.Dob,
                Gender = profile?.Gender,
                AddressDetail = profile?.AddressDetail,
                Roles = roles?.ToArray() ?? Array.Empty<string>(),
                WalletBalance = wallet?.Balance ?? 0m
            };

            return me;
        }
    }
}
