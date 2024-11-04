
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Integrations.AlphaVantage.Models;
using Accelerate.Foundations.Integrations.AlphaVantage.Services;

namespace Accelerate.Foundations.Integrations.AlphaVantage
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            // CONFIGS
            services.Configure<AlphaVantageConfiguration>(options =>
            {
                configuration.GetSection("AlphaVantageConfiguration").Bind(options);

                options.ApiKey = configuration["AlphaVantageApiKey"];
            });
            // SERVICES
            services.AddTransient<IAlphaVantageService, AlphaVantageService>();
        }
    }
}
