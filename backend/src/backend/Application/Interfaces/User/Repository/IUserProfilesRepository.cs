using Application.DTOs.UserProfile;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Repository
{
    public interface IUserProfilesRepository : IRepository<UserProfile, long>
    {
        Task<string?> GetIdentityNumberProfile(long userId, CancellationToken ct = default);
        Task<bool> IsIdentityNumberDuplicateAsync(string identityNumber, CancellationToken ct = default);
        Task<UserProfileDTO?> GetUserProfileWithVerify(long userId, CancellationToken ct = default);
    }
}
