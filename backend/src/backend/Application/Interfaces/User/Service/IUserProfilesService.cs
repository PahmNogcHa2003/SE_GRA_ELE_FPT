using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IUserProfilesService : IService<Domain.Entities.UserProfile, DTOs.UserProfileDTO, long>
    {
    }
}
