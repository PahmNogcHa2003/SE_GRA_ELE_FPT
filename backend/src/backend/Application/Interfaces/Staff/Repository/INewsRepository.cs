using Application.Common;
using Domain.Entities;
using Application.Interfaces.Base;

namespace Application.Interfaces.Staff.Repository
{
    // Kế thừa từ IRepository<News, long>
    public interface INewsRepository : IRepository<News, long>
    {
        // Phương thức mới để lấy News có kèm theo Author và Tags (dùng cho Update/Delete)
        Task<News?> GetNewsForUpdateAsync(long id);
        Task<News?> GetNewsDetailsAsync(long id); // Thêm phương thức này
    }
}