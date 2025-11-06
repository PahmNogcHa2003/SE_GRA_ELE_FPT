using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Base;

namespace Application.Interfaces.User.Service
{
    public interface INewsService : IService<Domain.Entities.News, DTOs.NewsDTO, long>
    {
        Task<IEnumerable<NewsDTO>> GetRelatedNewsAsync( long newsId,int limit = 5, CancellationToken ct = default);
    }
}
