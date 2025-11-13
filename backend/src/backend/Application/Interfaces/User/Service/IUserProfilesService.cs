using Application.DTOs.UserProfile;
using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IUserProfilesService : IService<Domain.Entities.UserProfile, UserProfileDTO, long>
    {
        Task<UserProfileDTO?> GetByUserIdAsync(long userId, CancellationToken ct = default);
        Task<bool> IsIdentityNumberDuplicateAsync(string identityNumber, CancellationToken ct = default);
        Task<UserProfileDTO?> UpdateBasicByUserIdAsync(long userId, UpdateUserProfileBasicDTO dto, CancellationToken ct = default);

    }
}
