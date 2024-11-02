
using Accelerate.Foundations.Users.Models.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Accelerate.Features.Accounts
{
    public static class Startup
    {
        public static async Task<IdentityResult> CreateRole(RoleManager<UsersRole> roleService, string name, string description)
        {
            try
            {
                var existinRole = await roleService.FindByNameAsync(name);
                if (existinRole != null) return null;

                var role = new UsersRole { Name = name, Description = description };
                return await roleService.CreateAsync(role);
            }
            catch (Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
                throw;
            }
        }
        public static async Task CreateAccountRoles(RoleManager<UsersRole> roleService)
        {
            await CreateRole(roleService, Foundations.Accounts.Constants.Roles.UserAccountIndividualName, Foundations.Accounts.Constants.Roles.UserAccountIndividualDescription);
            await CreateRole(roleService, Foundations.Accounts.Constants.Roles.UserAccountBusinessName, Foundations.Accounts.Constants.Roles.UserAccountBusinessDescription);
            
        }
        public static void ConfigureApp(WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var roleManager = (RoleManager<UsersRole>)scope.ServiceProvider.GetService(typeof(RoleManager<UsersRole>));
                //Task.FromResult(CreateGlobalAccounts(userManager));
                var task = Task.Run(async () => await CreateAccountRoles(roleManager));

                task.Wait();
            }

        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}
