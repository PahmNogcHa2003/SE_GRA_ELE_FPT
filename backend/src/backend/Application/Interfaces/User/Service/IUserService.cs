using Application.Common;
using Application.DTOs;
using Application.Interfaces.Base;
using Application.Services.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IUserService : IService<AspNetUser, UserDTO, long>
    {
        Task<List<UserDTO>> GetAllStatusAsync(CancellationToken ct = default);
    }
}
