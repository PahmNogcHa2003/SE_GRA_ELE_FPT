using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Repository
{
    public interface ICategoriesVehicleRepository : IRepository<Domain.Entities.CategoriesVehicle, long>
    {
    }
}
