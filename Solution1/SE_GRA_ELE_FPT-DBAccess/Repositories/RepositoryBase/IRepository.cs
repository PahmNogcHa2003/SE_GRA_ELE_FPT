using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SE_GRA_ELE_FPT_DBAccess.Repositories.RepositoryBase
{
    public interface IRepository<T> where T : class
    {       
        Task AddAsync(T entity);
        Task<T> GetAsync(string id);
        Task<T> GetIdAsync(int id);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null);
        Task<List<T>> GetWithPagingAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = null, int pageNumber = 1, int pageSize = 10);
        Task<T> GetEntityWithMaxKeyAsync<TKey>(Expression<Func<T, TKey>> keySelector, Expression<Func<T, bool>> filter = null, string includeProperties = null);
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> filter = null, string includeProperties = null);
        void Update(T entity);
        Task Remove(string id);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        IQueryable<T> GetAll(string includeProperties = null);

        T GetById(object id, params Expression<Func<T, object>>[] includes);
    }
}
