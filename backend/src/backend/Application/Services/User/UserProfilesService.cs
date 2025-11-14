using Application.DTOs.UserProfile;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Photo;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class UserProfilesService : GenericService<UserProfile, UserProfileDTO, long>, IUserProfilesService
    {
        private readonly IUserProfilesRepository _userProfilesRepository;
        private readonly IPhotoService _photoService;

        public UserProfilesService(
            IRepository<UserProfile, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            IUserProfilesRepository userProfilesRepository,
            IPhotoService photoService) : base(repo, mapper, uow)
        {
            _userProfilesRepository = userProfilesRepository;
            _photoService = photoService;
        }

        /// <summary>
        /// Lấy UserProfile theo userId (gọi trực tiếp từ repository)
        /// </summary>
        public Task<UserProfileDTO?> GetByUserIdAsync(long userId, CancellationToken ct = default)
        => _userProfilesRepository.GetUserProfileWithVerify(userId, ct);

        public async Task<bool> IsIdentityNumberDuplicateAsync(string identityNumber, CancellationToken ct = default)
        {
            return await _userProfilesRepository.IsIdentityNumberDuplicateAsync(identityNumber, ct);
        }

        public async Task<UserProfileDTO?> UpdateBasicByUserIdAsync(
        long userId, UpdateUserProfileBasicDTO dto, CancellationToken ct = default)
        {
            var entity = await _repo.Query().FirstOrDefaultAsync(x => x.UserId == userId, ct);
            if (entity == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                entity.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
                entity.AvatarUrl = dto.AvatarUrl;

            if (!string.IsNullOrWhiteSpace(dto.EmergencyName))
                entity.EmergencyName = dto.EmergencyName;

            if (!string.IsNullOrWhiteSpace(dto.EmergencyPhone))
                entity.EmergencyPhone = dto.EmergencyPhone;

            entity.AddressDetail = dto.AddressDetail ?? entity.AddressDetail;

            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<UserProfileDTO>(entity);
        }
        /// <summary>
        /// upload avatar
        public async Task<UserProfileDTO?> UpdateAvatarAsync(long userId, IFormFile file, CancellationToken ct = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File avatar không hợp lệ");
            var entity = await _repo.Query().FirstOrDefaultAsync(x => x.UserId == userId, ct);
            if (entity == null) return null;
            if (!string.IsNullOrWhiteSpace(entity.AvatarPublicId))
            {
                try
                {
                    await _photoService.DeletePhotoAsync(entity.AvatarPublicId);
                }
                catch
                {
                    Console.WriteLine("Xoá ảnh cũ thất bại, có thể ảnh không tồn tại trên Cloudinary");
                }
            }
            var upload = await _photoService.AddPhotoAsync(file);
            if (upload == null || string.IsNullOrWhiteSpace(upload.Url))
                throw new Exception("Upload ảnh thất bại");
            entity.AvatarUrl = upload.Url;
            entity.AvatarPublicId = upload.PublicId;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<UserProfileDTO>(entity);
        }

    }
}
