using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Pipelines;
using Accelerate.Features.Content.Consumers;
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Foundations.Account.Models.Entities;
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
            services.AddSingleton<IDataCreateEventPipeline<AccountUser>, AccountUserCreatedPipeline>();
            services.AddSingleton<IDataCreateCompletedEventPipeline<AccountUser>, EmptyCreatedCompletedPipeline<AccountUser>>();
            services.AddSingleton<IDataUpdateEventPipeline<AccountUser>, AccountUserUpdatedPipeline>();
            services.AddSingleton<IDataUpdateCompletedEventPipeline<AccountUser>, EmptyUpdatedCompletedPipeline<AccountUser>>();
            services.AddSingleton<IDataDeleteEventPipeline<AccountUser>, AccountUserDeletedPipeline>();
            services.AddSingleton<IDataDeleteCompletedEventPipeline<AccountUser>, EmptyDeletedCompletedPipeline<AccountUser>>();

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
