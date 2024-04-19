
using Accelerate.Foundations.Integrations.AzureSecrets.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelerate.Foundations.Integrations.AzureSecrets
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddSingleton<IAzureKeyVaultService, AzureKeyVaultService>();
        }
    }
}
