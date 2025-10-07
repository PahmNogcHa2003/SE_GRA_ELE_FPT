using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
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
    public class UserService : GenericService<AspNetUser, UserDTO, long>, IUserService
    {
        public UserService(IRepository<AspNetUser, long> repo, IMapper mapper, IUnitOfWork uow)
        : base(repo, mapper, uow) { }

        public async Task<List<UserDTO>> GetAllStatusAsync(CancellationToken ct = default)
        {
            var entities = await _repo.Query()
                .AsNoTracking()
                .ToListAsync(ct);  // <--- thêm dòng này để thực thi truy vấn

            return _mapper.Map<List<UserDTO>>(entities);
        }

    }
}
