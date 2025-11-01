using Application.Common;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
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
    }
}

