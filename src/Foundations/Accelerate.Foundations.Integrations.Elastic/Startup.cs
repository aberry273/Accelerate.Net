
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;

namespace Accelerate.Foundations.Integrations.Elastic
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            services.Configure<ElasticConfiguration>(options =>
            {
                configuration.GetSection("ElasticConfiguration").Bind(options);

                options.ApiKey = configuration["ElasticApiKey"];
                options.CloudId = configuration["ElasticCloudId"];
            });
            // SERVICES
        }
    }
}
