using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Users.Services
{
    public interface IUsersUserService
    {
        Task<string> GenerateTwoFactorTokenAsync(UsersUser user, string tokenProvider = "Email");
        Task<UsersUser?> FindByNameAsync(string name);
        Task<UsersUser?> FindByEmailAsync(string email);
        Task<UsersUser?> FindByClaimAsync(ClaimsPrincipal principle);
        Task<UsersUser?> FindByIdAsync(string id);
        Task<UsersUser?> FindByNameAsync(string loginProvider, string providerKey);
        Task<int> Delete(UsersUser entity);
        Task<bool> UserInRole(UsersUser user, string roleName);
        Task<IdentityResult?> CreateAsync(UsersUser user);
        Task<IdentityResult?> CreateAsync(UsersUser user, string password);
        Task<IdentityResult> CreateUser(string email, string domain);
        Task<IdentityResult> CreateUser(string username, string email, string domain, string password);
        Task<IdentityResult> AddRole(string name, ICollection<UsersRoleClaim> roleClaims = null);
        Task<IdentityResult> AddUserToRole(UsersUser user, string roleName);
    }
}
