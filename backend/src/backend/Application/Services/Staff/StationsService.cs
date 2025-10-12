using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Staff
{
    public class StationsService : GenericService<Station, StationDTO, long>, IStationsService
    {
        public StationsService(IRepository<Station, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }

       
    }
}
