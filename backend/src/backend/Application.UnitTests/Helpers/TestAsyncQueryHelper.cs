using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Application.UnitTests.Helpers
{
    public static class TestAsyncQueryHelper
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
        {
            return new TestAsyncEnumerable<T>(source);
        }

        private class TestAsyncEnumerable<T> : IQueryable<T>, IAsyncEnumerable<T>
        {
            private readonly IQueryable<T> _query;
            public TestAsyncEnumerable(IEnumerable<T> source) => _query = source.AsQueryable();

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
                => new TestAsyncEnumerator<T>(_query.GetEnumerator());

            public Type ElementType => _query.ElementType;
            public Expression Expression => _query.Expression;
            public IQueryProvider Provider => new TestAsyncQueryProvider<T>(_query.Provider);

            public IEnumerator<T> GetEnumerator() => _query.GetEnumerator();
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _query.GetEnumerator();
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
            public T Current => _inner.Current;
            public ValueTask DisposeAsync() { _inner.Dispose(); return default; }
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
        }

        private class TestAsyncQueryProvider<T> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }

            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(_inner.CreateQuery<T>(expression));
            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(_inner.CreateQuery<TElement>(expression));
            public object Execute(Expression expression) => _inner.Execute(expression);
            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new TestAsyncEnumerable<TResult>(_inner.CreateQuery<TResult>(expression));
            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Task.FromResult(Execute<TResult>(expression));

            TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
