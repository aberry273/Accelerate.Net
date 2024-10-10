using Accelerate.Foundations.Account.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account.Services
{
    public interface IAccountUserService
    {
        Task<AccountUser?> FindByNameAsync(string name);
        Task<AccountUser?> FindByEmailAsync(string email);
        Task<AccountUser?> FindByIdAsync(string id);
        Task<AccountUser?> FindByNameAsync(string loginProvider, string providerKey);
        Task<IdentityResult?> CreateAsync(AccountUser user);
        Task<IdentityResult?> CreateAsync(AccountUser user, string password);
    }
}
