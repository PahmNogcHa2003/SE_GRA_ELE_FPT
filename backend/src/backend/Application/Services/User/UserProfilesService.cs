using Application.DTOs.UserProfile;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
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
    }
}
