using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Pipelines;
using Accelerate.Features.Account.Services;
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Accelerate.Features.Account
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAccountViewService, AccountViewService>();

            Foundations.EventPipelines.Startup.ConfigurePipelineServices<AccountUser, AccountUserCreatedPipeline, AccountUserUpdatedPipeline, AccountUserDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<AccountUser>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<AccountUser, IAccountBus>(services); 
        }
    }
}
