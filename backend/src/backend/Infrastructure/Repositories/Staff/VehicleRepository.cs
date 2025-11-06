using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    public class VehicleRepository : BaseRepository<Vehicle, long>, IVehicleRepository
    {
        public VehicleRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        public async Task<long> GetStationVehicle(long vehicleId)
        {
            var vehicle = await _dbContext.Vehicles
                .FirstOrDefaultAsync(x => x.Id == vehicleId);

            if (vehicle == null)
                throw new InvalidOperationException($"Vehicle with ID {vehicleId} not found.");

            return (long)vehicle.StationId;
        }

        public async Task<Vehicle?> GetVehicleWithCategoryAsync(long vehicleId)
        {
            return await _dbContext.Vehicles
                  .Include(v => v.Category)
                  .FirstOrDefaultAsync(v => v.Id == vehicleId);
        }
    }
}
