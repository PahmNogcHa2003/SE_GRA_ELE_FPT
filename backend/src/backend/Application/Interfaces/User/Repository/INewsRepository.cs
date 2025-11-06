using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.User.Repository
{
    public interface INewsRepository : IRepository<Domain.Entities.News, long>
    {
        Task<News?> GetNewsForUpdateAsync(long id);
        Task<News?> GetNewsDetailsAsync(long id);
    }
}
