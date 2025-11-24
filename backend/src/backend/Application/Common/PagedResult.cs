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
        public List<T> Items { get; }
        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;

        // Constructor
        public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            Items = new List<T>(items);
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }

        // Async version dùng IQueryable (EF)
        public static async Task<PagedResult<T>> FromQueryableAsync(
            IQueryable<T> query,
            int page,
            int pageSize,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            try
            {
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
                return new PagedResult<T>(Array.Empty<T>(), 0, page, pageSize);
            }
        }

        // Đồng bộ version cho List<T> in-memory
        public static PagedResult<T> FromList(IEnumerable<T> list, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var itemsList = list.ToList();
            var totalCount = itemsList.Count;
            var pageItems = itemsList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>(pageItems, totalCount, page, pageSize);
        }
    }
}
