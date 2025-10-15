using Application.Common;
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    public class NewsRepository : BaseRepository<News, long>, INewsRepository
    {
        public NewsRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// Lấy chi tiết một bài News để hiển thị (read-only).
        /// </summary>
        public async Task<News?> GetNewsDetailsAsync(long id)
        {
            // Dùng AsNoTracking() vì đây là thao tác đọc để tối ưu hiệu suất.
            // Include() cả Author và Tags để có đầy đủ thông tin chi tiết.
            return await _dbSet.AsNoTracking()
                .Include(n => n.Author)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Lấy một bài News để chuẩn bị cho việc Cập nhật hoặc Xóa.
        /// </summary>
        public async Task<News?> GetNewsForUpdateAsync(long id)
        {
            // Entity được trả về sẽ được DbContext theo dõi (tracking).
            return await _dbSet
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
    }
}