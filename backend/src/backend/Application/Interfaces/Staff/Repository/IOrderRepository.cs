using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.Staff.Repository
{
    public interface IOrderRepository : IRepository<Order, long> { }
}
