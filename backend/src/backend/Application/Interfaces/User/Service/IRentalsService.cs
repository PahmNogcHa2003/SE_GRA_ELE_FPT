using Application.DTOs.Rental;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IRentalsService : IService<Domain.Entities.Rental, DTOs.RentalDTO, long>
    {
        Task EndRentalAsync(long rentalId, EndRentalRequestDTO endRentalDto);

    }
}
