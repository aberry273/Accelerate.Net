
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Integrations.AzureStorage.Services;

namespace Accelerate.Foundations.Integrations.AzureStorage
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            services.Configure<AzureStorageConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.SectionName).Bind(options);

                options.ContainerName = configuration[Constants.Config.ContainerName];
                options.AccessKey = configuration[Constants.Config.AccessKey];
                options.ConnectionString = configuration[Constants.Config.ConnectionString];
            });
            // SERVICES
            services.AddSingleton<IBlobStorageService, BlobStorageService>();
        }
    }
}
