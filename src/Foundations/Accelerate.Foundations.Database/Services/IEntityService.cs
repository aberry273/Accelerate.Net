
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Accelerate.Foundations.Database.Services
{
    public interface IEntityService<T>
    {
        void ClearDatabase();
        int Count(Expression<Func<T, bool>> expression);
        Task<int> CopyRange(IEnumerable<T> entities);
        Task<int> DeleteRange(IEnumerable<T> entities);
        Task<int> UpdateRange(IEnumerable<T> entities);
        Task<int> Delete(T entity);
        Task<int> AddRange(IEnumerable<T> entities);
        Task<Guid?> CreateWithGuid(T entity);
        Task<int> Create(T entity);
        Task<T?> CreateAndReturn(T entity);
        Task<int> SaveChangesAsync();
        T Get(Guid guid);
        Task<T> GetAsync(Guid id);
        Task<int> Update(T entity);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression, int? skip = 0, int? take = 10);
        IEnumerable<T> FindAndInclude<TProperty>(Expression<Func<T, bool>> expression, Expression<Func<T, TProperty>> includeExpression, int? skip, int? take);
    }
}