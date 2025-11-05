using Application.DTOs;
using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface INewsService : IService<Domain.Entities.News, DTOs.NewsDTO, long>
    {
    }
}
