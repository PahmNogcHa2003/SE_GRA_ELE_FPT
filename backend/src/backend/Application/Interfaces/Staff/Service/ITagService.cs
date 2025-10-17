using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface ITagService : IService<Domain.Entities.Tag, DTOs.TagDTO, long>
    {
    }
}
