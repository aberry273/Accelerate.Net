
using Accelerate.Foundations.EventPipelines.Pipelines;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Accelerate.Foundations.EventPipeline
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            //services.AddTransient<IContentPostElasticService, ContentPostElasticService>();

            //services.AddSingleton<IContentPostCreatedPipeline, ContentPostCreatedPipeline>();
            /*
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
            */
        }
    }
}
