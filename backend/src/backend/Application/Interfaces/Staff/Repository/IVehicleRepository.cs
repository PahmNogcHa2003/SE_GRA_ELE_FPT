using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Repository
{
    public interface IVehicleRepository : IRepository<Domain.Entities.Vehicle, long>
    {
        Task<long> GetStationVehicle(long vehicleId);
        Task<Vehicle?> GetVehicleWithCategoryAsync(long vehicleId);
    }
}
