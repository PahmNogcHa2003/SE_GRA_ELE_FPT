using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : GenericService<AspNetUser, UserDTO, long>
    {
        public UserService(IRepository<AspNetUser, long> repo, IMapper mapper, IUnitOfWork uow)
        : base(repo, mapper, uow) { }
    }
}
