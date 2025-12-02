using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UnitTests.Helpers
{
    public static class AsyncTestHelper
    {
        // Đổi tên tránh trùng
        public static IQueryable<T> AsQueryableForTest<T>(this IEnumerable<T> source)
        {
            return source.AsQueryable();
        }

        // Mock ToListAsync cho unit test
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        {
            return Task.FromResult(query.ToList());
        }
    }
}
