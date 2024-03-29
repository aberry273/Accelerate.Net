using Accelerate.Features.Content.Consumers;
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit;
using System;
using System.Reflection;

namespace Accelerate.Features.Content
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddTransient<IContentViewService, ContentViewService>();

            services.AddSingleton<IDataCreateEventPipeline<ContentPostEntity>, ContentPostCreatedPipeline>();
            services.AddSingleton<IDataUpdateEventPipeline<ContentPostEntity>, ContentPostUpdatedPipeline>();
            services.AddSingleton<IDataDeleteEventPipeline<ContentPostEntity>, ContentPostDeletedPipeline>();
            
            // CONSUMERS
            services.AddMassTransit<IContentBus>(x =>
            {
                x.AddConsumer<DataCreateConsumer<ContentPostEntity, IContentBus>>();
                x.AddConsumer<DataCreateCompleteConsumer<ContentPostEntity>>();

                x.AddConsumer<DataUpdateConsumer<ContentPostEntity, IContentBus>>();
                x.AddConsumer<DataUpdateCompleteConsumer<ContentPostEntity>>();

                x.AddConsumer<DataDeleteConsumer<ContentPostEntity, IContentBus>>();
                x.AddConsumer<DataDeleteCompleteConsumer<ContentPostEntity>>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<ContentPostEntity, IContentBus>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<ContentPostEntity>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<ContentPostEntity, IContentBus>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<ContentPostEntity>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<ContentPostEntity, IContentBus>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<ContentPostEntity>>(context);
                    });
                });
            });
        }
    }
}
