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
    public interface IRentalsService
    {
        Task<bool> CreateRentalAsync(CreateRentalDTO createRentalDTO);
        Task<bool> EndRentalAsync(EndRentalRequestDTO endRentalDto);
    }
}
