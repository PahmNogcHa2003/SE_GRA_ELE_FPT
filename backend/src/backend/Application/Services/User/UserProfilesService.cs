using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Repository;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class UserProfilesService : GenericService<UserProfile, UserProfileDTO, long>, IUserProfilesService
    {
        public UserProfilesService(IRepository<UserProfile, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }

        public async Task<UserProfileDTO?> GetByUserIdAsync(long userId, CancellationToken ct = default)
        {
            var entity = await _repo.Query()
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt > p.CreatedAt ? p.UpdatedAt : p.CreatedAt)
                .FirstOrDefaultAsync(ct);

            return entity is null ? null : _mapper.Map<UserProfileDTO>(entity);
        }
    }
}
