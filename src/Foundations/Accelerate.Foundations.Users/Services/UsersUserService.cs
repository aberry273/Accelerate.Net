using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Users.Models;
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
using static Accelerate.Foundations.Communication.Constants.Templates;
using System.Security.Claims;
using Twilio.Rest;
using static Azure.Core.HttpHeader;
using System.Data;

namespace Accelerate.Foundations.Users.Services
{
    public class UsersUserService : IEntityService<UsersUser>, IUsersUserService
    {
        protected readonly ILogger _logger;
        UserManager<UsersUser> _userManager;
        RoleManager<UsersRole> _roleManager;
        IEntityService<UsersProfile> _profileService;
        public UsersUserService(
            ILogger<UsersUserService> logger,
            IEntityService<UsersProfile> profileService,
            UserManager<UsersUser> userManager,
            RoleManager<UsersRole> roleManager
        )
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _profileService = profileService;
        }

        public async Task<UsersUser?> FindByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public async Task<UsersUser?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<UsersUser?> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<UsersUser?> FindByClaimAsync(ClaimsPrincipal principle)
        {
            return await _userManager.GetUserAsync(principle);
        }


        public async Task<UsersUser?> FindByNameAsync(string loginProvider, string providerKey)
        {
            return await _userManager.FindByLoginAsync(loginProvider, providerKey);
        }

        public async Task<IdentityResult?> CreateAsync(UsersUser user)
        {
            return await _userManager.CreateAsync(user);
        }
        public async Task<IdentityResult?> CreateAsync(UsersUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
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
        public async Task<int> CopyRange(IEnumerable<UsersUser> entities)
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
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public async Task<int> DeleteRange(IEnumerable<UsersUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.DeleteAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Delete(UsersUser entity)
        {
            try
            {
                var result = _userManager.DeleteAsync(entity);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> Update(UsersUser entity)
        {
            try
            {
                var result = _userManager.UpdateAsync(entity);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> UpdateRange(IEnumerable<UsersUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.UpdateAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Update), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        private async Task CreateUserProfile(UsersUser user)
        {
            if(user == null)
            {
                Foundations.Common.Services.StaticLoggingService.Log("CreateUserProfile user null");
                return;
            }
            var profileId = await _profileService.CreateWithGuid(new UsersProfile() { UserId = user.Id });
            // Get user and update with profile id
            user.UsersProfileId = profileId.GetValueOrDefault();
            await _userManager.UpdateAsync(user);
        }
        public virtual EntityEntry<UsersUser> Add(UsersUser entity)
        {
            try
            {
                var result = _userManager.CreateAsync(entity); 
                var user = _userManager.FindByNameAsync(entity.UserName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<string> GenerateTwoFactorTokenAsync(UsersUser user, string tokenProvider = "Email")
        {
            return await _userManager.GenerateTwoFactorTokenAsync(user, tokenProvider);
        }
        public async Task<IdentityResult> CreateUser(string email, string domain)
        {
            var user = new UsersUser { UserName = email, Email = email, Domain = domain, Status = UsersUserStatus.Active };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // Create profile
                var profileId = await _profileService.CreateWithGuid(new UsersProfile() { UserId = user.Id });
                // Get user and update with profile id
                user = await _userManager.FindByNameAsync(email);
                user.UsersProfileId = profileId.GetValueOrDefault();
                await _userManager.UpdateAsync(user);
            }
            return result;
        }
        public async Task<IdentityResult> CreateUser(string username, string email, string domain, string password)
        {
            var user = new UsersUser { UserName = username, Email = email, Domain = domain, Status = UsersUserStatus.Active };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                // Create profile
                var profileId = await _profileService.CreateWithGuid(new UsersProfile() { UserId = user.Id });
                // Get user and update with profile id
                user = await _userManager.FindByNameAsync(username);
                user.UsersProfileId = profileId.GetValueOrDefault();
                await _userManager.UpdateAsync(user);
            }
            return result;
        }

        public async Task<bool> UserInRole(UsersUser user, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null) throw new Exception($"Role: {roleName} not found");

            return await _userManager.IsInRoleAsync(user, role.Name);
        }
        public async Task<IdentityResult> AddRole(string name, ICollection<UsersRoleClaim> roleClaims = null)
        {
            var role = new UsersRole { Name = name, RoleClaims = roleClaims };
            return await _roleManager.CreateAsync(role);
        }
        public async Task<IdentityResult> AddUserToRole(UsersUser user, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null) throw new Exception($"Role: {roleName} not found");

            if (await _userManager.IsInRoleAsync(user, role.Name))
            {
                return await _userManager.AddToRoleAsync(user, roleName);
            }
            return new IdentityResult();
        }
        public async Task<int> Create(UsersUser entity)
        {
            try
            {
                var result = _userManager.CreateAsync(entity);
                var user = await _userManager.FindByNameAsync(entity.UserName);
                await this.CreateUserProfile(user);
                return result.Result.Succeeded ? 1 : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<Guid?> CreateWithGuid(UsersUser entity)
        {
            try
            {
                var id = Guid.NewGuid();
                entity.Id = id;
                var creationResult = await _userManager.CreateAsync(entity);
                var user = await _userManager.FindByNameAsync(entity.UserName);
                await this.CreateUserProfile(user);
                var result = creationResult.Succeeded ? 1 : 0;
                return result > 0 ? id : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<int> AddRange(IEnumerable<UsersUser> entities)
        {
            try
            {
                var result = await entities.SelectAsync(x => _userManager.CreateAsync(x));
                return result.Count(x => x.Succeeded);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Create), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public UsersUser Get(Guid id)
        {
            try
            {
                var result = _userManager.FindByIdAsync(id.ToString());
                return result?.Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public async Task<UsersUser> GetAsync(Guid id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Get), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public int Count(Expression<Func<UsersUser, bool>> expression)
        {
            try
            {
                return _userManager.Users.Count(expression);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public IEnumerable<UsersUser> Find(Expression<Func<UsersUser, bool>> expression, int? skip = 0, int? take = 10)
        {
            try
            {
                return _userManager.Users.Where(expression).OrderByDescending(x => x.CreatedOn).Skip(skip ?? 0).Take(take ?? int.MaxValue).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Find), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }
        public IEnumerable<UsersUser> FindAndInclude<TProperty>(Expression<Func<UsersUser, bool>> expression, Expression<Func<UsersUser, TProperty>> includeExpression, int? skip, int? take)
        {
            throw new NotImplementedException();
        }
        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(UsersUser entity)
        {
            try
            {
                await _userManager.CreateAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format(Database.Constants.Exceptions.Service.ExceptionMessage, nameof(UsersUserService), nameof(this.Delete), ex.Message));
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        public Task<UsersUser?> CreateAndReturn(UsersUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
