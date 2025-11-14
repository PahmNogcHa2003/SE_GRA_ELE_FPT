using Application.DTOs.UserProfile;
using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories.User
{
    public class UserProfileRepository : BaseRepository<UserProfile, long>, IUserProfilesRepository
    {
        public UserProfileRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        public async Task<string?> GetIdentityNumberProfile(long userId, CancellationToken ct = default)
        {
            return await _dbContext.UserProfiles
                .AsNoTracking()
                .Where(up => up.UserId == userId && up.NumberCard != null)
                .Select(up => up.NumberCard)
                .FirstOrDefaultAsync();
        }
        public async Task<bool> IsIdentityNumberDuplicateAsync(
            string identityNumber,
            CancellationToken ct = default)
        {
            var query = _dbContext.UserProfiles
                    .AsNoTracking()
                    .Where(p => p.NumberCard == identityNumber);
            return await query.AnyAsync();
        }

        public async Task<UserProfileDTO?> GetUserProfileWithVerify(long userId, CancellationToken ct = default)
        {
            var q =
                 from up in _dbContext.Set<UserProfile>().AsNoTracking()
                 join u in _dbContext.Set<AspNetUser>().AsNoTracking()
                     on up.UserId equals u.Id
                 join k in _dbContext.Set<KycForm>().AsNoTracking()
                     on up.UserId equals k.UserId into kycGroup
                 from kyc in kycGroup
                     .OrderByDescending(x => x.SubmittedAt)
                     .Take(1)
                     .DefaultIfEmpty()
                 where up.UserId == userId
                 select new UserProfileDTO
                 {
                     UserId = userId,
                     FullName = up.FullName,
                     Email = u.Email,
                     PhoneNumber = u.PhoneNumber,
                     Dob = up.Dob,
                     Gender = up.Gender,
                     AvatarUrl = up.AvatarUrl,
                     EmergencyName = up.EmergencyName,
                     EmergencyPhone = up.EmergencyPhone,
                     ProvinceCode = up.ProvinceCode,
                     ProvinceName = up.ProvinceName,
                     WardCode = up.WardCode,
                     WardName = up.WardName,
                     AddressDetail = up.AddressDetail,

                     NumberCard = up.NumberCard,
                     PlaceOfOrigin = up.PlaceOfOrigin,
                     PlaceOfResidence = up.PlaceOfResidence,
                     IssuedDate = up.IssuedDate,
                     ExpiryDate = up.ExpiryDate,
                     IssuedBy = up.IssuedBy,

                     CreatedAt = up.CreatedAt,
                     UpdatedAt = up.UpdatedAt,

                     IsVerify = kyc != null ? kyc.Status : "None"
                 };
            return await q.FirstOrDefaultAsync(ct);
        }
  
    }
}
