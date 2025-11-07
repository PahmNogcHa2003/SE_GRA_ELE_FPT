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

namespace Infrastructure.Repositories.User
{
    public class UserProfileRepository : BaseRepository<UserProfile, long>, IUserProfilesRepository
    {
        public UserProfileRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        public async Task<string> GetIdentityNumberProfile(long userId)
        {
            var numberCard = await _dbContext.UserProfiles
                .Where(up => up.UserId == userId && up.NumberCard != null)
                .Select(up => up.NumberCard)
                .FirstOrDefaultAsync(); 
            return numberCard;
        }

        public async Task<UserProfileDTO?> GetUserProfileWithVerify(long userId)
        {
            var userProfile = await Query()
                .Include(up => up.User)                       
                    .ThenInclude(u => u.KycForms)            
                .Where(up => up.UserId == userId)
                .OrderByDescending(up => up.UpdatedAt > up.CreatedAt ? up.UpdatedAt : up.CreatedAt)
                .FirstOrDefaultAsync();

            if (userProfile == null)
                return null;

            // Lấy KYC mới nhất
            var latestKyc = userProfile.User.KycForms
                .OrderByDescending(k => k.SubmittedAt)
                .FirstOrDefault();

            // Mapping sang DTO
            return new UserProfileDTO
            {
                UserId = userProfile.UserId,
                FullName = userProfile.FullName,
                NumberCard = userProfile.NumberCard,
                Dob = userProfile.Dob,
                Gender = userProfile.Gender,
                AvatarUrl = userProfile.AvatarUrl,
                AddressDetail = userProfile.AddressDetail,
                IsVerify = latestKyc?.Status  
            };
        }
    }
}
