using Application.DTOs.Station;
using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface IStationsService : IService<Domain.Entities.Station, StationDTO, long>
    {
    }
}
