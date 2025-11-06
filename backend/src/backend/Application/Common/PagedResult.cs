using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common
{
    public class PagedResult<T>
    {
        // THAY ĐỔI 1: Chuyển từ IQueryable<T> thành List<T> để đảm bảo query đã được thực thi
        public List<T> Items { get; }

        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        // THÊM MỚI: Các thuộc tính tiện ích giúp hiển thị trên UI dễ dàng hơn
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        // Constructor nhận IEnumerable và chuyển thành List
        public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            Items = new List<T>(items);
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }

        // SỬA LẠI: Toàn bộ phương thức tĩnh FromQueryableAsync
        public static async Task<PagedResult<T>> FromQueryableAsync<T>(
            IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
                // Nếu đã bị hủy thì trả nhanh
                if (ct.IsCancellationRequested)
                    return new PagedResult<T>(Array.Empty<T>(), 0, page, pageSize);

                var totalCount = await query.CountAsync(ct);
                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(ct);

                return new PagedResult<T>(items, totalCount, page, pageSize);
            }
            catch (OperationCanceledException)
            {
                // Request bị cancel (user chuyển trang, refetch…) → trả rỗng, không log lỗi
                return new PagedResult<T>(Array.Empty<T>(), 0, page, pageSize);
            }
        }

    }
}