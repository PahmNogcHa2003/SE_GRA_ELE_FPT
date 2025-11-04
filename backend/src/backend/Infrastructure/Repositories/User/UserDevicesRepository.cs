using Application.Interfaces.User.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    public class UserDevicesRepository : BaseRepository<UserDevice, long>, IUserDevicesRepository
    {
        public UserDevicesRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
    }
}
