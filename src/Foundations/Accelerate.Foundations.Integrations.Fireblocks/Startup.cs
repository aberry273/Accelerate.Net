
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Integrations.Fireblocks.Services;
using Accelerate.Foundations.Integrations.Fireblocks.Models;

namespace Accelerate.Foundations.Integrations.Fireblocks
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            services.Configure<FireblocksConfiguration>(options =>
            {
                configuration.GetSection("FireblocksConfiguration").Bind(options);

                options.ApiKey = configuration["ApiKey"];
            });
            // SERVICES
            services.AddTransient<IFireblocksService, FireblocksService>();
        }
    }
}
