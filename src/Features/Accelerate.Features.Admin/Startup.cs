
using Accelerate.Features.Admin.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Operations.Models.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Accelerate.Features.Admin
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IAdminViewService, AdminViewService>();
            services.AddTransient<IAdminBaseEntityViewService<OperationsJobEntity>, AdminJobEntityViewService>();
            services.AddTransient<IAdminBaseEntityViewService<OperationsActionEntity>, AdminActionEntityViewService>();
            services.AddTransient<IAdminBaseEntityViewService<AccountUser>, AdminUserEntityViewService>();

        }
    }
}
