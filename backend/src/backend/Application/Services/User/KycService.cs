using Application.DTOs.Kyc;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Ocr;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Quan trọng: Thêm using này
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class KycService : GenericService<KycForm, KycDTO, long>, IKycService
    {
        private readonly IPhotoRepository _photoAccessor;
        private readonly ILogger<KycService> _logger;
        private readonly IServiceProvider _serviceProvider; // Thêm IServiceProvider để tạo scope cho tác vụ nền

        public KycService(
            IRepository<KycForm, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            IPhotoRepository photoAccessor,
            ILogger<KycService> logger,
            IServiceProvider serviceProvider) : base(repo, mapper, uow) // Thêm serviceProvider vào constructor
        {
            _photoAccessor = photoAccessor;
            _logger = logger;
            _serviceProvider = serviceProvider; // Gán giá trị
        }

        public async Task<bool> SubmitKycImagesAsync(long userId, IFormFile frontImage, IFormFile backImage)
        {
            // Bước 1 & 2: Kiểm tra, upload ảnh và tạo bản ghi KYC ban đầu
            _logger.LogInformation("Bắt đầu quá trình gửi yêu cầu KYC cho User ID: {UserId}", userId);

            var existingKyc = await _repo.Query().FirstOrDefaultAsync(k =>
                k.UserId == userId &&
                (k.Status == KycStatus.Pending || k.Status == KycStatus.Approved));

            if (existingKyc != null)
            {
                _logger.LogWarning("User ID: {UserId} đã cố gắng gửi yêu cầu KYC trong khi đã có một yêu cầu tồn tại với trạng thái {Status}", userId, existingKyc.Status);
                throw new BadRequestException("Bạn đã có một yêu cầu xác thực đang được xử lý hoặc đã được duyệt.");
            }

            var frontUploadResult = await _photoAccessor.AddPhotoAsync(frontImage);
            var backUploadResult = await _photoAccessor.AddPhotoAsync(backImage);

            if (frontUploadResult == null || backUploadResult == null)
            {
                _logger.LogError("Upload ảnh lên Cloudinary thất bại cho User ID: {UserId}", userId);
                throw new InvalidOperationException("Upload ảnh không thành công, vui lòng thử lại.");
            }

            var kycFormEntity = new KycForm
            {
                UserId = userId,
                IdFrontUrl = frontUploadResult.Url,
                IdBackUrl = backUploadResult.Url,
                Status = KycStatus.Pending,
                SubmittedAt = DateTimeOffset.UtcNow
            };

            // Sử dụng repo từ base class để thêm entity
            await _repo.AddAsync(kycFormEntity);
            await _uow.SaveChangesAsync(); // Lưu bản ghi ban đầu

            _logger.LogInformation("Gửi yêu cầu KYC thành công. Bắt đầu xử lý OCR trong nền. ID yêu cầu: {KycId}", kycFormEntity.Id);

            // Bước 3: Kích hoạt tác vụ nền để xử lý OCR và không chờ nó hoàn thành
            // API sẽ trả về ngay lập tức sau dòng này.
            _ = Task.Run(() => ProcessOcrInBackground(kycFormEntity.Id, frontUploadResult.Url, backUploadResult.Url));

            return true;
        }

        private async Task ProcessOcrInBackground(long kycId, string frontUrl, string backUrl)
        {
            // Khi chạy tác vụ nền, chúng ta cần tạo một "scope" mới để lấy các service
            // như DbContext, vì scope của request ban đầu đã kết thúc.
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedUow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var scopedKycRepo = scope.ServiceProvider.GetRequiredService<IKycRepository>();
                var scopedOcrRepo = scope.ServiceProvider.GetRequiredService<IOcrRepository>();
                var scopedMapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var scopedLogger = scope.ServiceProvider.GetRequiredService<ILogger<KycService>>();

                try
                {
                    scopedLogger.LogInformation("[Background] Bắt đầu đọc thông tin từ ảnh cho KYC ID: {KycId}", kycId);

                    var ocrDataDto = await scopedOcrRepo.ReadIdCardInfoAsync(frontUrl, backUrl);

                    // Phải lấy lại entity trong scope mới để Entity Framework theo dõi
                    var kycToUpdate = await scopedKycRepo.GetByIdAsync(kycId);

                    if (kycToUpdate != null && ocrDataDto != null)
                    {
                        scopedMapper.Map(ocrDataDto, kycToUpdate);
                        kycToUpdate.Status = KycStatus.Pending; // Giữ nguyên trạng thái chờ duyệt

                        scopedKycRepo.Update(kycToUpdate);
                        await scopedUow.SaveChangesAsync(); // Lưu dữ liệu OCR vào DB

                        scopedLogger.LogInformation("[Background] Cập nhật thông tin từ OCR thành công cho KYC ID: {KycId}", kycId);
                    }
                    else
                    {
                        scopedLogger.LogWarning("[Background] OCR không trả về dữ liệu hoặc không tìm thấy KYC ID: {KycId}. Yêu cầu sẽ được xử lý thủ công.", kycId);
                    }
                }
                catch (Exception ex)
                {
                    scopedLogger.LogError(ex, "[Background] Lỗi nghiêm trọng trong quá trình xử lý OCR cho KYC ID: {KycId}", kycId);
                }
            }
        }
    }
}

