using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Pipelines;
using Accelerate.Features.Account.Services;
using Accelerate.Features.Content.Consumers;
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.Communication.Services;
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

            services.AddTransient<IDataCreateEventPipeline<AccountUser>, AccountUserCreatedPipeline>();
            services.AddTransient<IDataCreateCompletedEventPipeline<AccountUser>, EmptyCreatedCompletedPipeline<AccountUser>>();
            services.AddTransient<IDataUpdateEventPipeline<AccountUser>, AccountUserUpdatedPipeline>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<AccountUser>, EmptyUpdatedCompletedPipeline<AccountUser>>();
            services.AddTransient<IDataDeleteEventPipeline<AccountUser>, AccountUserDeletedPipeline>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<AccountUser>, EmptyDeletedCompletedPipeline<AccountUser>>();

            // CONSUMERS
            services.AddMassTransit<IAccountBus>(x =>
            {
                x.AddConsumer<DataCreateConsumer<AccountUser, IAccountBus>>();
                x.AddConsumer<DataCreateCompleteConsumer<AccountUser>>();

                x.AddConsumer<DataUpdateConsumer<AccountUser, IAccountBus>>();
                x.AddConsumer<DataUpdateCompleteConsumer<AccountUser>>();

                x.AddConsumer<DataDeleteConsumer<AccountUser, IAccountBus>>();
                x.AddConsumer<DataDeleteCompleteConsumer<AccountUser>>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<AccountUser, IAccountBus>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<AccountUser>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<AccountUser, IAccountBus>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<AccountUser>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<AccountUser, IAccountBus>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<AccountUser>>(context);
                    });
                });
            });
        }
    }
}
