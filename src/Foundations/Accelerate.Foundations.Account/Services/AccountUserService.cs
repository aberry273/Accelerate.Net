using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Accelerate.Foundations.Database.Services;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Accelerate.Foundations.Common.Extensions;

namespace Accelerate.Foundations.Account.Services
{
    public class AccountUserService : IEntityService<AccountUser>, IAccountUserService
    {
        protected readonly ILogger _logger;
        UserManager<AccountUser> _userManager;
        public AccountUserService(
            ILogger<AccountUserService> logger,
            IEntityService<AccountProfile> profileService,
            UserManager<AccountUser> userManager
        )
        {
            _logger = logger;
            _userManager = userManager;
        }
        public void InitializeAdmin()
        {
            Task.FromResult(Foundations.Account.Startup.InitializeDefaultAdmin(_userManager));
        }
        public void ClearDatabase()
        {
            try
            {
                _userManager.Users.ExecuteDelete();
            }
            catch (Exception ex)
            {
                _userManager.Users.ExecuteDelete();
            }
        }
        public async Task<int> CopyRange(IEnumerable<AccountUser> entities)
        {
            try
            {
                var item = entities.ToList();
                item.ForEach(x =>
                {
                    x.Id = new Guid();
                    x.CreatedOn = DateTime.Now;
                    _userManager.CreateAsync(x);
                });
                var result = await item.SelectAsync(x => _userManager.CreateAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<int> DeleteRange(IEnumerable<AccountUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.DeleteAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Delete(AccountUser entity)
        {
            try
            {
                var result = _userManager.DeleteAsync(entity);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Update(AccountUser entity)
        {
            try
            {
                var result = _userManager.UpdateAsync(entity);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> UpdateRange(IEnumerable<AccountUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.UpdateAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public virtual EntityEntry<AccountUser> Add(AccountUser entity)
        {
            try
            {
                var result = _userManager.CreateAsync(entity);
                var user = _userManager.FindByNameAsync(entity.UserName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Create(AccountUser entity)
        {
            try
            {
                var result = _userManager.CreateAsync(entity);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<Guid?> CreateWithGuid(AccountUser entity)
        {
            try
            {
                var id = Guid.NewGuid();
                entity.Id = id;
                var creationResult = _userManager.CreateAsync(entity);
                var result = creationResult.Result.Succeeded ? 1 : 0;
                return result > 0 ? id : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> AddRange(IEnumerable<AccountUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.CreateAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public AccountUser Get(Guid id)
        {
            try
            {
                var result = _userManager.FindByIdAsync(id.ToString());
                return result?.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<AccountUser> GetAsync(Guid id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public int Count(Expression<Func<AccountUser, bool>> expression)
        {
            try
            {
                return _userManager.Users.Count(expression);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public IEnumerable<AccountUser> Find(Expression<Func<AccountUser, bool>> expression, int? skip = 0, int? take = 10)
        {
            try
            {
                return _userManager.Users.Where(expression).OrderByDescending(x => x.CreatedOn).Skip(skip ?? 0).Take(take ?? int.MaxValue).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public IEnumerable<AccountUser> FindAndInclude<TProperty>(Expression<Func<AccountUser, bool>> expression, Expression<Func<AccountUser, TProperty>> includeExpression, int? skip, int? take)
        {
            throw new NotImplementedException();
        }
        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(AccountUser entity)
        {
            try
            {
                await _userManager.CreateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(AccountUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

    }
}
