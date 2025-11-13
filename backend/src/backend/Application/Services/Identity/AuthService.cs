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
using System.Data;
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

        // === REGISTER ===
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model, CancellationToken ct = default)
        {
            // 1) Kiểm tra đơn giản
            if (model.Password != model.ConfirmPassword)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Password and Confirm Password do not match."
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Email already exists."
                };
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                var existingPhoneUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == model.PhoneNumber, ct);
                if (existingPhoneUser != null)
                {
                    return new AuthResponseDTO { IsSuccess = false, Message = "Phone number already exists." };
                }
            }
            var identityExist = await _profileService.IsIdentityNumberDuplicateAsync(model.IdentityNumber, ct);
            if (identityExist)
            {
                return new AuthResponseDTO { IsSuccess = false, Message = "Identity number already exists." };
            }
            var now = DateTimeOffset.UtcNow;
            var user = new AspNetUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber?.ToString(),
                CreatedDate = now
            };

            await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
            try
            {
                // 2) Tạo user
                var createResult = await _userManager.CreateAsync(user, model.Password);
                if (!createResult.Succeeded)
                {
                    var msg = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    await _unitOfWork.RollbackTransactionAsync(tx, ct);
                    return new AuthResponseDTO { IsSuccess = false, Message = msg };
                }

                // 3) Gán role
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    var msg = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    await _unitOfWork.RollbackTransactionAsync(tx, ct);
                    return new AuthResponseDTO { IsSuccess = false, Message = msg };
                }

                // 4) Tạo hồ sơ người dùng
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

                // 5) Tạo ví người dùng
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

                // 6) Lưu và commit
                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync(tx, ct);

                return new AuthResponseDTO
                {
                    IsSuccess = true,
                    Message = "Registration successful."
                };
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

        // === LOGIN ===
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO model, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials." };

            var check = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials." };

            // Lưu thông tin thiết bị (nếu có)
            if (!string.IsNullOrEmpty(model.DeviceId))
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

            return new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = jwt,
                Roles = roles.ToList()
            };
        }

        // === GET CURRENT USER INFO ===
        private long? GetUserIdAsLong(ClaimsPrincipal userPrincipal)
        {
            var idStr = _userManager.GetUserId(userPrincipal);
            return long.TryParse(idStr, out var id) ? id : null;
        }

        public async Task<MeDTO?> GetMeAsync(ClaimsPrincipal userPrincipal, CancellationToken ct = default)
        {
            var userId = GetUserIdAsLong(userPrincipal);
            if (userId == null) return null;

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var profile = await _profileService.GetByUserIdAsync(user.Id, ct);
            var wallet = await _userWalletService.GetByUserIdAsync(user.Id, ct);

            return new MeDTO
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = profile?.FullName,
                AvatarUrl = profile?.AvatarUrl,
                CreatedDate = user.CreatedDate,
                Gender = profile?.Gender,
                AddressDetail = profile?.AddressDetail,
                Roles = roles.ToArray(),
                WalletBalance = wallet?.Balance ?? 0m
            };
        }
    }
}
