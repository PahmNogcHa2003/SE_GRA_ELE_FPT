using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Repository
{
    public interface IUserRepository : IRepository<Domain.Entities.AspNetUser, long>
    {
        IQueryable<AspNetUser> QueryAllUserByStatus();
    }
}
