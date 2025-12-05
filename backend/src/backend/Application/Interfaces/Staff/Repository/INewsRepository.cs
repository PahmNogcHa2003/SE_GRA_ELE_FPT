using Application.Common;
using Domain.Entities;
using Application.Interfaces.Base;

namespace Application.Interfaces.Staff.Repository
{
    // Kế thừa từ IRepository<News, long>
    public interface INewsRepository : IRepository<News, long>
    {
        
    }
}