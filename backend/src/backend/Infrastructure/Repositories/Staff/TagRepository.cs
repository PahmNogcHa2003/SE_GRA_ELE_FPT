using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    public class TagRepository : BaseRepository<Tag, long>, ITagRepository
    {
        public TagRepository(HolaBikeContext dbContext) : base(dbContext)
        {
           
        }
    }
}
