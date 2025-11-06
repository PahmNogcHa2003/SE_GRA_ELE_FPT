using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs;
using Application.Interfaces.Base;

namespace Application.Interfaces.User.Service
{
    public interface IStationsService : IService<Domain.Entities.Station, DTOs.StationDTO, long>
    {
        Task<PagedResult<StationDTO>> GetNearbyPagedAsync(double lat, double lng, double radiusKm, int page, int pageSize, CancellationToken ct = default);
        Task<IEnumerable<StationDTO>> GetAllAsync(CancellationToken ct = default);
    }
}
