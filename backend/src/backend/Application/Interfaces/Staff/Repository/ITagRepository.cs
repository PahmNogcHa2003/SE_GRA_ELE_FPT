using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Repository
{
    public interface ITagRepository : IRepository<Domain.Entities.Tag, long>
    {
        Task<List<Tag>> FindOrCreateTagsAsync(IEnumerable<string> tagNames);
    }
}
