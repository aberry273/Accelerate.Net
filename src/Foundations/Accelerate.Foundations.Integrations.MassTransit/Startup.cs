using Accelerate.Foundations.Integrations.MassTransit;
using Accelerate.Foundations.Integrations.MassTransit.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Accelerate.Foundations.Integrations.MassTransit
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GettingStartedConsumer>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        e.ConfigureConsumer<GettingStartedConsumer>(context);
                    });
                });
            });

            services.AddMassTransitHostedService();
        }
    }
}
