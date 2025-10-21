using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.UserDevice;
using Application.Interfaces;
using Application.Interfaces.Identity;
using Application.Interfaces.User.Service; 
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<AspNetUser> _userManager;
    private readonly SignInManager<AspNetUser> _signInManager;
    private readonly IJwtTokenService _tokenService;
    private readonly IUserProfilesService _profileService; 
    private readonly IUserDevicesService _userDevicesService;
    private readonly IUnitOfWork _unitOfWork;
   

    public AuthService(
        UserManager<AspNetUser> userManager,
        SignInManager<AspNetUser> signInManager,
        IJwtTokenService tokenService,
        IUserProfilesService userProfilesService,
        IUserDevicesService userDevicesService,
        IUnitOfWork unitOfWork
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _profileService = userProfilesService;
        _userDevicesService = userDevicesService;
        _unitOfWork = unitOfWork;
    }

    // --- Phương thức ĐĂNG KÝ ---
    public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model)
    {
        // 1. Kiểm tra Mật khẩu trùng khớp (Nên được thực hiện trong DTO Validation)
        if (model.Password != model.ConfirmPassword)
        {
            return new AuthResponseDTO { IsSuccess = false, Message = "Password and Confirm Password do not match." };
        }

        // 2. Kiểm tra Email tồn tại
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return new AuthResponseDTO { IsSuccess = false, Message = "Email already exists" };

        // 3. Chuẩn bị AspNetUser
        var user = new AspNetUser
        {
            Email = model.Email,
            UserName = model.Email,
            PhoneNumber = model.PhoneNumber.ToString(),
            CreatedDate = DateTimeOffset.UtcNow
        };

        // Bắt đầu Transaction
        using var transaction = _unitOfWork.BeginTransaction();

        try
        {
            // 4. Tạo AspNetUser
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }

            // 5. Chuẩn bị UserProfileDTO
            var userProfileDto = new UserProfileDTO
            {
                // UserId là long
                UserId = user.Id,

                Dob = model.DateOfBirth,
                EmergencyName = model.FullName,

                EmergencyPhone = model.PhoneNumber,
                NumberCard = model.IdentityNumber,

                Gender = model.Gender,
                FullName = model.FullName,
                AddressDetail = model.Address,
                ProvinceCode = model.ProvinceId,
                ProvinceName = model.ProvinceName,
                WardCode = model.WardId,
                WardName = model.WardName,

                // Gán thời gian tạo/cập nhật
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            await _userManager.AddToRoleAsync(user, "User");

            // 6. Tạo UserProfile thông qua Service
            await _profileService.CreateAsync(userProfileDto);

            // 7. Commit Transaction
            await _unitOfWork.CommitAsync();

            return new AuthResponseDTO { IsSuccess = true, Message = "Registration successful" };
        }
        catch (Exception ex)
        {
            // 8. Rollback (xảy ra tự động khi Dispose nếu chưa commit, nhưng nên thêm log)
            // Log the exception (ex) here!

            return new AuthResponseDTO
            {
                IsSuccess = false,
                Message = "Registration failed due to an internal server error. Please check server logs."
            };
        }
    }

    public async Task<AuthResponseDTO> LoginAsync(LoginDTO model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials" };
        }

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPassword.Succeeded)
        {
            return new AuthResponseDTO { IsSuccess = false, Message = "Invalid credentials" };
        }
        
        //user device
        var userDeviceDto = new CreateUserDeviceDTO
        {
            UserId = user.Id,
            DeviceId = model.DeviceId,
            PushToken = model.PushToken,
            Platform = model.Platform
        };

        await _userDevicesService.HandleDeviceLoginAsync(userDeviceDto);

        // 3. Tạo JWT Token
        var jwt = await _tokenService.GenerateJwtTokenAsync(user);

        return new AuthResponseDTO
        {
            IsSuccess = true,
            Message = "Login successful",
            Token = jwt
        };
    }
}
