
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Accelerate.Foundations.Database.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Accelerate.Foundations.Database.Services
{
    public class EntityService<T> : IEntityService<T> where T : BaseEntity
    {
        private readonly BaseContext<T> _dbContext;
        private readonly ILogger _logger;
        public EntityService(
            BaseContext<T> dbContext,
            ILogger<EntityService<T>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> CopyRange(IEnumerable<T> entities)
        {
            try
            {
                var item = entities.ToList();
                item.ForEach(x =>
                {
                    x.Id = new Guid();
                    x.CreatedOn = DateTime.Now;
                    _dbContext.Add<T>(x);
                });
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<int> DeleteRange(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.RemoveRange(entities);
                _dbContext.ChangeTracker.DetectChanges();
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Delete(T entity)
        {
            try
            {
                _dbContext.Remove<T>(entity);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Update(T entity)
        {
            try
            {
                _dbContext.Update(entity);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> UpdateRange(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.UpdateRange(entities);
                _dbContext.ChangeTracker.DetectChanges();
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public virtual EntityEntry<T> Add(T entity)
        {
            try
            {
                return _dbContext.Add<T>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Create(T entity)
        {
            try
            {
                await _dbContext.AddAsync<T>(entity);
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<Guid?> CreateWithGuid(T entity)
        {
            try
            {
                var id = Guid.NewGuid();
                entity.Id = id;
                await _dbContext.AddAsync<T>(entity);
                var result = await _dbContext.SaveChangesAsync();
                return result > 0 ? id : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> AddRange(IEnumerable<T> entities)
        {
            try
            {
                _dbContext.AddRange(entities);
                _dbContext.ChangeTracker.DetectChanges();
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public T Get(Guid id)
        {
            try
            {
                return _dbContext.Entities.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<T> GetAsync(Guid id)
        {
            try
            {
                return await _dbContext.Entities.FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        } 
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression, int? skip = 0, int? take = 10)
        {
            try
            {
                return _dbContext.Entities.Where(expression).OrderByDescending(x => x.CreatedOn).Skip(skip ?? 0).Take(take ?? int.MaxValue).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public IEnumerable<T> FindAndInclude<TProperty>(Expression<Func<T, bool>> expression, Expression<Func<T, TProperty>> includeExpression, int? skip, int? take)
        {
            try
            {
                return _dbContext.Entities.Where(expression).Include(includeExpression).OrderByDescending(x => x.CreatedOn).Skip(skip ?? 0).Take(take ?? int.MaxValue).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _dbContext.AddAsync<T>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Constants.Exceptions.Service.ExceptionMessage, nameof(EntityService<T>), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}
