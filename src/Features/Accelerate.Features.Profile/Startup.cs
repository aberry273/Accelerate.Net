using Accelerate.Features.Profile.Models;
using Accelerate.Features.Profile.Services; 
using Accelerate.Foundations.Users.EventBus;
using Accelerate.Foundations.Users.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace Accelerate.Features.Profile
{
    public static class Startup
    {  
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProfileViewService, ProfileViewService>();

            //Foundations.EventPipelines.Startup.ConfigurePipelineServices<UsersUser, UsersUserCreatedPipeline, UsersUserUpdatedPipeline, UsersUserDeletedPipeline>(services);
           // Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<UsersUser>(services);
           // Foundations.EventPipelines.Startup.ConfigureMassTransitServices<UsersUser, IUsersBus>(services); 
        }
    }
}
