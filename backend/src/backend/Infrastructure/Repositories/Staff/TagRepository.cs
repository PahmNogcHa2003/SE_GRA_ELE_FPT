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

        public async Task<List<Tag>> FindOrCreateTagsAsync(IEnumerable<string> tagNames)
        {
            var normalizedTagNames = tagNames.Select(n => n.Trim().ToLower()).Distinct().ToList();

            // 1. Tìm tất cả các tag đã có trong DB
            var existingTags = await _dbSet
                .Where(t => normalizedTagNames.Contains(t.Name.ToLower()))
                .ToListAsync();

            var existingTagNames = existingTags.Select(t => t.Name.ToLower());

            // 2. Lọc ra những tên tag chưa có trong DB
            var newTagNames = normalizedTagNames.Except(existingTagNames);

            // 3. Tạo entity mới cho những tag chưa có
            var newTags = newTagNames.Select(name => new Tag { Name = name }).ToList();

            // 4. Thêm các tag mới vào DbContext để chuẩn bị INSERT
            if (newTags.Any())
            {
                await _dbSet.AddRangeAsync(newTags);
            }

            // 5. Gộp danh sách tag đã có và tag mới tạo lại rồi trả về
            return existingTags.Concat(newTags).ToList();
        }
    }
}
