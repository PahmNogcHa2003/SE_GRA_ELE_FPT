using Application.DTOs;
using Application.DTOs.New;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface INewsService : IService<Domain.Entities.News, NewsDTO, long>
    {
        Task<NewsDTO?> UpdateBannerAsync(long newsId, IFormFile file, CancellationToken ct = default);
    }
}
