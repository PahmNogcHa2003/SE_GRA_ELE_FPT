using Application.DTOs.Rental;
using Application.DTOs.RentalHistory;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IRentalsService
    {
        Task<VehicleDetailDTO> GetVehicleByCode(RequestVehicleDTO requestVehicleDTO);
        Task<long> CreateRentalAsync(CreateRentalDTO createRentalDTO , CancellationToken ct = default);
        Task<bool> EndRentalAsync(EndRentalRequestDTO endRentalDto, CancellationToken ct = default);
        Task<IReadOnlyList<RentalHistoryDTO>> GetMyRentalHistoryAsync(CancellationToken ct = default);
    }
}
