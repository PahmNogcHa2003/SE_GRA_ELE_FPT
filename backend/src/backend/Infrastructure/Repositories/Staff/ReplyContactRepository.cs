// Nằm trong: Infrastructure/Repositories/Staff
using Application.Interfaces.Staff.Repository;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Staff
{
    // Triển khai Interface (IReplyContactRepository)
    public class ReplyContactRepository : BaseRepository<Contact, long>, IReplyContactRepository
    {
        // Hàm khởi tạo (constructor) đã có từ file của bạn
        public ReplyContactRepository(HolaBikeContext dbContext) : base(dbContext)
        {
        }

        // Triển khai phương thức kiểm tra tùy chỉnh
        public async Task<bool> IsContactRepliedAsync(long id, CancellationToken cancellationToken = default)
        {
            // Chúng ta sử dụng _dbSet (DbSet<Contact>) kế thừa từ BaseRepository

            // Dựa trên Entity của bạn, cách kiểm tra tin cậy nhất là:
            // 1. Status == "Replied"
            // 2. HOẶC ReplyContent có nội dung
            return await _dbSet
                .AnyAsync(c => c.Id == id &&
                               (c.Status == "Replied" || !string.IsNullOrEmpty(c.ReplyContent)),
                          cancellationToken);
        }

        // Các phương thức GetByIdAsync, Update, Add...
        // đã được kế thừa tự động từ BaseRepository<Contact, long>
    }
}