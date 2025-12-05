using Application.Common;
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
    public class StationsRepository : BaseRepository<Station, long>, IStationsRepository
    {
        public StationsRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }
        public async Task<string?> GetStationNameByIdAsync(long stationId)
        {
            var station =  await _dbContext.Stations
                  .Where(s => s.Id == stationId)
                  .Select(s => s.Name)
                  .FirstOrDefaultAsync();
            return station;
        }
    }
}

