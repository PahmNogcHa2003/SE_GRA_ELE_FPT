using Application.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Interfaces.Base
{
    public interface IService<TEntity, TDto, TKey> where TEntity : class
    {
        /// <summary>
        /// Lấy một đối tượng theo ID.
        /// </summary>
        Task<TDto?> GetAsync(TKey id, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách các đối tượng có phân trang, tìm kiếm, lọc và sắp xếp.
        /// </summary>
        /// <param name="page">Số trang hiện tại.</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang.</param>
        /// <param name="searchQuery">Từ khóa tìm kiếm trên nhiều trường (logic được định nghĩa ở lớp service con).</param>
        /// <param name="filterField">Tên thuộc tính cần lọc chính xác.</param>
        /// <param name="filterValue">Giá trị cần lọc.</param>
        /// <param name="sortOrder">Chuỗi sắp xếp (ví dụ: "Name asc", "Capacity desc").</param>
        /// <param name="ct">Cancellation token.</param>
        Task<PagedResult<TDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? searchQuery = null,
            string? filterField = null,
            string? filterValue = null,
            string? sortOrder = null,
            CancellationToken ct = default);

        /// <summary>
        /// Tạo một đối tượng mới.
        /// </summary>
        Task<TDto> CreateAsync(TDto dto, CancellationToken ct = default);

        /// <summary>
        /// Cập nhật một đối tượng đã có theo ID.
        /// </summary>
        Task UpdateAsync(TKey id, TDto dto, CancellationToken ct = default);

        /// <summary>
        /// Xóa một đối tượng theo ID.
        /// </summary>
        Task DeleteAsync(TKey id, CancellationToken ct = default);
    }
}