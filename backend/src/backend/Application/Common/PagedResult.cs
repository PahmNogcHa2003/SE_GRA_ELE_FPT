using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Common
{
    public class PagedResult<T>
    {
        public IQueryable<T> Items { get; set; } = Enumerable.Empty<T>().AsQueryable();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PagedResult() { }

        public PagedResult(IQueryable<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }

        public static async Task<PagedResult<T>> FromQueryableAsync(
            IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var totalCount = await query.CountAsync();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return new PagedResult<T>(items, totalCount, page, pageSize);
        }
    }
}
