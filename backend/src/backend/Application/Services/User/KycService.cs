using Application.DTOs.Kyc;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            _logger.LogInformation("Bắt đầu quá trình gửi yêu cầu KYC cho User ID: {UserId}", userId);

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
                Status = KycStatus.Approved,
                SubmittedAt = DateTimeOffset.UtcNow
            };

            // Sử dụng repo từ base class để thêm entity
            await _repo.AddAsync(kycFormEntity);
            await _uow.SaveChangesAsync(); // Lưu bản ghi ban đầu

            _logger.LogInformation("Gửi yêu cầu KYC thành công. Bắt đầu xử lý OCR trong nền. ID yêu cầu: {KycId}", kycFormEntity.Id);


            return true;
        }

    }
}

