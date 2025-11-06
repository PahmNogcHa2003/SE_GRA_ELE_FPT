using Application.DTOs.Kyc;
using Application.Interfaces;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class KycService : IKycService
    {
        private readonly IIdentificationPhotoRepository _photoAccessor;
        private readonly ILogger<KycService> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserProfilesRepository _userProfilesRepository;
        private readonly IKycRepository _kycRepository;

        public KycService(
            IIdentificationPhotoRepository photoAccessor,
            ILogger<KycService> logger,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IUserProfilesRepository userProfilesRepository,
            IKycRepository kycRepository
        )
        {
            _photoAccessor = photoAccessor;
            _logger = logger;
            _uow = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userProfilesRepository = userProfilesRepository;
            _kycRepository = kycRepository;
        }

        public async Task<bool> CreateKycAsync(CreateKycRequestDTO createKycRequestDTO)
        {
            try
            {
                if (createKycRequestDTO == null)
                {
                    _logger.LogWarning("CreateKycRequestDTO null");
                    return false;
                }

                if (string.IsNullOrEmpty(createKycRequestDTO.JsonData))
                {
                    _logger.LogInformation("Chuỗi JsonData trống");
                    return false;
                }

                // Tách CCCD từ chuỗi gửi lên
                var parts = createKycRequestDTO.JsonData.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    _logger.LogInformation("Chuỗi JsonData không hợp lệ");
                    return false;
                }

                var identityNumber = parts[0];
                _logger.LogInformation("Số CCCD: {IdentityNumber}", identityNumber);

                // Lấy UserId từ Claims
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    _logger.LogWarning("Không tìm thấy User ID trong Claims.");
                    throw new UnauthorizedAccessException("User chưa đăng nhập.");
                }

                // Lấy CCCD hiện tại của user
                var existingIdentityNumber = await _userProfilesRepository.GetIdentityNumberProfile(userId);

                if (existingIdentityNumber == null)
                {
                    _logger.LogInformation("User chưa cập nhật số CCCD trong profile.");
                    return false;
                }

                // So sánh CCCD
                if (!identityNumber.Equals(existingIdentityNumber.ToString()))
                {
                    _logger.LogInformation("Số CCCD không khớp với profile của user.");
                    return false;
                }

                // Upload ảnh CCCD lên Cloudinary (private)
                var frontUploadResult = await _photoAccessor.AddPhotoAsync(createKycRequestDTO.FrontImage);
                var backUploadResult = await _photoAccessor.AddPhotoAsync(createKycRequestDTO.BackImage);

                // Tạo KYC record
                var kycRecord = new KycForm
                {
                    NumberCard = identityNumber,
                    IdFrontUrl = frontUploadResult.PublicId,
                    IdBackUrl = backUploadResult.PublicId,
                    Status = KycStatus.Approved,
                    SubmittedAt = DateTimeOffset.UtcNow,
                    UserId = userId
                };

                await _kycRepository.AddAsync(kycRecord);
                await _uow.SaveChangesAsync();

                _logger.LogInformation("KYC created successfully for UserId: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo yêu cầu KYC");
                throw;
            }
        }
    }
}
