using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IRentalsService : IService<Domain.Entities.Rental, DTOs.RentalDTO, long>
    {
    }
}
