
using Accelerate.Features.Rates.Services;
using Accelerate.Features.Admin.Services;
using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Accelerate.Foundations.Rates.Models.Entities;

namespace Accelerate.Features.Rates
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
            await CreateRole(roleService, Foundations.Portal.Constants.Roles.RatesQuote, "Internal portal role for individual conversion rates access");
            
        }
        public static void ConfigureApp(WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var roleManager = (RoleManager<UsersRole>)scope.ServiceProvider.GetService(typeof(RoleManager<UsersRole>));
                //Task.FromResult(CreateGlobalRates(userManager));
                var task = Task.Run(async () => await CreateAccountRoles(roleManager));

                task.Wait();
            }

        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IRatesBaseEntityViewService<RatesConversionQuoteEntity>, RatesConversionQuoteViewService>();
        }
    }
}
