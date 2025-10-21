using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class RentalsService : GenericService<Rental, RentalDTO, long>, IRentalsService
    {
        public RentalsService(IRepository<Rental, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
    }
}
