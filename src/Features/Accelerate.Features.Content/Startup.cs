using Accelerate.Features.Content.Consumers;
using Accelerate.Features.Content.Models.Data;
using Accelerate.Features.Content.Pipelines;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using MassTransit;
using System;

namespace Accelerate.Features.Content
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddTransient<IContentPostElasticService, ContentPostElasticService>();

            services.AddSingleton<IContentPostCreatedPipeline, ContentPostCreatedPipeline>();
            // CONSUMERS
            services.AddMassTransit<IContentBus>(x =>
            {
                x.AddConsumer<ContentPostCreateConsumer>();
                x.AddConsumer<ContentPostCreateCompleteConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        e.ConfigureConsumer<ContentPostCreateConsumer>(context);
                        e.ConfigureConsumer<ContentPostCreateCompleteConsumer>(context);
                    });
                });
            });
        }
    }
}
