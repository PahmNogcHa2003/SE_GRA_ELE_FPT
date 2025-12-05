using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common;
using Application.DTOs.Rental.Manage;

namespace Application.Interfaces.Staff.Service
{
    public interface IManageRentalsService
    {
        Task<PagedResult<RentalListDTO>> GetPagedAsync(
           int page,
           int pageSize,
           RentalFilterDTO filter,
           CancellationToken ct = default);

        Task<RentalDetailDTO> GetDetailAsync(long id, CancellationToken ct = default);
    }
}
