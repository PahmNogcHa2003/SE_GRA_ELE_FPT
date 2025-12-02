using Application.DTOs;
using Application.DTOs.Auth;
using Application.DTOs.Email;
using Application.DTOs.UserDevice;
using Application.DTOs.UserProfile;
using Application.DTOs.Wallet;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Interfaces.Identity;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.User.Service;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using System.Threading;

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
        private  readonly IEmailRepository _emailService;

        public AuthService(
            UserManager<AspNetUser> userManager,
            SignInManager<AspNetUser> signInManager,
            IJwtTokenService tokenService,
            IUserProfilesService userProfilesService,
            IUserDevicesService userDevicesService,
            IWalletService walletService,
            IUnitOfWork unitOfWork,
            IUserWalletService userWalletService,
            IEmailRepository emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _profileService = userProfilesService;
            _userDevicesService = userDevicesService;
            _unitOfWork = unitOfWork;
            _walletService = walletService;
            _userWalletService = userWalletService;
            _emailService = emailService;
        }

        // === REGISTER ===
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO model, CancellationToken ct = default)
        {
            if (!IsStrongPassword(model.Password))
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt."
                };
            }
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

                // 7) Send email register success 

                var emailDto = new EmailResponseRegister();
                emailDto.HtmlContent = emailDto.HtmlContent.Replace("{{UserName}}", user.UserName);

                var mailData = new MailData
                {
                    EmailToId = user.Email,
                    EmailToName = user.UserName,
                    EmailSubject = "Chào mừng đến với hệ thống!",
                    EmailBody = emailDto.HtmlContent
                };

                var emailResult =  await _emailService.SendAsync(mailData);

                if (emailResult == false)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Lỗi gửi email đăng ký thành công."
                    };
                }

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
        public async Task<AuthResponseDTO> ChangePasswordAsync(
            ClaimsPrincipal userPrincipal,
            ChangePasswordDTO model,
            CancellationToken ct = default)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu hiện tại không đúng."
                };
            }
            if (model.CurrentPassword == model.NewPassword)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu mới phải khác với mật khẩu cũ."
                };
            }
            if(model.CurrentPassword == null || model.NewPassword == null || model.ConfirmNewPassword == null)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu không được để trống ."
                };
            }
            if (!IsStrongPassword(model.NewPassword))
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt."
                };
            }
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Mật khẩu mới và xác nhận mật khẩu không trùng khớp."
                };
            }
            if (user == null)
            {
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = "Không xác định được người dùng."
                };
            }
            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword
            );
            if (!result.Succeeded)
            {
                var msg = string.Join(", ", result.Errors.Select(e => e.Description));
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = msg
                };
            }
            return new AuthResponseDTO
            {
                IsSuccess = true,
                Message = "Đổi mật khẩu thành công."
            };
        }
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;
            if (!password.Any(char.IsUpper))
                return false;
            if (!password.Any(char.IsLower))
                return false;
            if (!password.Any(char.IsDigit))
                return false;
            if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(ch)))
                return false;
            return true;
        }

        public async Task<AuthResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO model, CancellationToken ct = default)
        {
            try
            {
                // Find user
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Email không tồn tại trong hệ thống."
                    };
                }

                // Validate strong password (theo yêu cầu UI: ≥10 kí tự, hoa, đặc biệt)
                if (!IsStrongPassword(model.NewPassword))
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường, số và ký tự đặc biệt."
                    };
                }

                // Confirm password
                if (model.NewPassword != model.ConfirmPassword)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu xác nhận không trùng khớp."
                    };
                }

                // Decode token
                var decodedToken = Uri.UnescapeDataString(model.Token);

                // Reset password
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = $"Reset password failed: {errors}"
                    };
                }

                return new AuthResponseDTO
                {
                    Message = "Đặt lại mật khẩu thành công.",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error in ForgotPasswordAsync", ex);
            }
        }

        public async Task<AuthResponseDTO> SendEmailResetPassword(EmailForgotPassword model, CancellationToken ct = default)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.EmailForForgotPassword);
                if (user == null)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Email not found."
                    };
                }

                // 1. Generate reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // 2. Encode token and email
                var encodedToken = Uri.EscapeDataString(token);
                var encodedEmail = Uri.EscapeDataString(user.Email);

                // 3. Construct reset URL
                var resetUrl = $"https://localhost:5001/resetpassword?email={encodedEmail}&token={encodedToken}";

                // 4. Load template and inject reset link
                var emailDto = new EmailResponseForgotPassword();
                emailDto.HtmlContent = emailDto.HtmlContent.Replace("{{ResetLink}}", resetUrl);

                // 5. Prepare email
                var mailData = new MailData
                {
                    EmailToId = user.Email,
                    EmailToName = user.Email,
                    EmailSubject = "Reset your password",
                    EmailBody = emailDto.HtmlContent
                };

                // 6. Send email
                var emailResult = await _emailService.SendAsync(mailData);

                // 7. Ensure result is consistent
                if (emailResult == false)
                {
                    return new AuthResponseDTO
                    {
                        IsSuccess = false,
                        Message = "Failed to send reset password email."
                    };
                }

                return new AuthResponseDTO
                {
                    Message = "Gửi link thành công.",
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                // Never throw raw exceptions; return failure instead
                return new AuthResponseDTO
                {
                    IsSuccess = false,
                    Message = $"Error sending reset password email: {ex.Message}"
                };
            }
        }


    }
}

