using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Pipelines;
using Accelerate.Features.Account.Services;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Users.EventBus;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Users.Services;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.EventPipelines.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Accelerate.Features.Account
{
    public static class Startup
    {
        public static async Task<IdentityResult> CreateUser(UserManager<UsersUser> userService, Guid id, string name, string password)
        {
            try
            {
                var existingUser = userService.Users.FirstOrDefault(x => x.Id == id);
                if (existingUser != null)
                    return null;

                var user = new UsersUser { Id = id, UserName = name, Email = $"{name}@internal", Domain = Foundations.Users.Constants.Domains.Internal, EmailConfirmed = true, Status = UsersUserStatus.Active };
                return await userService.CreateAsync(user, password);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static async Task CreateGlobalAccounts(UserManager<UsersUser> userService)
        {
            await CreateUser(userService, Foundations.Common.Constants.Global.GlobalAdminMedia, "Admin", "Password1!!");
            await CreateUser(userService, Foundations.Common.Constants.Global.GlobalAdminAccounts, "AccountAdmin", "Password1!!");
            await CreateUser(userService, Foundations.Common.Constants.Global.GlobalAdminContent, "ContentAdmin", "Password1!!");
            await CreateUser(userService, Foundations.Common.Constants.Global.GlobalAdminMedia, "MediaAdmin", "Password1!!");
            await CreateUser(userService, Foundations.Common.Constants.Global.GlobalAdminOperations, "OperationsAdmin", "Password1!!");
        }
        public static void ConfigureApp(WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                //Resolve ASP .NET Core Identity with DI help
                var userManager = (UserManager<UsersUser>)scope.ServiceProvider.GetService(typeof(UserManager<UsersUser>));
                //Task.FromResult(CreateGlobalAccounts(userManager));
                var task = Task.Run(async () => await CreateGlobalAccounts(userManager));

                task.Wait();
                //Task.FromResult(CreateUser(userManager, Foundations.Common.Constants.Global.GlobalAdminMedia, "Admin", "Password1!!"));
                // Task.FromResult(CreateGlobalAccounts(userManager));
            }

        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAccountViewService, AccountViewService>();

            Foundations.EventPipelines.Startup.ConfigurePipelineServices<UsersUser, UsersUserCreatedPipeline, UsersUserUpdatedPipeline, UsersUserDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<UsersUser>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<UsersUser, IUsersBus>(services); 
        }
    }
}
