using Application.DTOs.UserProfile;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class UserProfilesService : GenericService<UserProfile, UserProfileDTO, long>, IUserProfilesService
    {
        private readonly IUserProfilesRepository _userProfilesRepository;

        public UserProfilesService(
            IRepository<UserProfile, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            IUserProfilesRepository userProfilesRepository) : base(repo, mapper, uow)
        {
            _userProfilesRepository = userProfilesRepository;
        }

        /// <summary>
        /// Lấy UserProfile theo userId (gọi trực tiếp từ repository)
        /// </summary>
        public async Task<UserProfileDTO?> GetByUserIdAsync(long userId, CancellationToken ct = default)
        {
            if (userId <= 0)
                return null;

            // Gọi repo lấy DTO kèm KYC
            var userProfileDto = await _userProfilesRepository.GetUserProfileWithVerify(userId);

            return userProfileDto;
        }
        public async Task<bool> IsIdentityNumberDuplicateAsync(string identityNumber, CancellationToken ct = default)
        {
            // 2. Service CHỈ CẦN GỌI Repository
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

    }
}
