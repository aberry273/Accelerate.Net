using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Users.Services
{
    public interface IUsersUserService
    {
        Task<UsersUser?> FindByNameAsync(string name);
        Task<UsersUser?> FindByEmailAsync(string email);
        Task<UsersUser?> FindByIdAsync(string id);
        Task<UsersUser?> FindByNameAsync(string loginProvider, string providerKey);
        Task<IdentityResult?> CreateAsync(UsersUser user);
        Task<IdentityResult?> CreateAsync(UsersUser user, string password);
    }
}
