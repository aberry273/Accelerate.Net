
using Accelerate.Foundations.Media.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelerate.Foundations.Media
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddSingleton<IMediaService, MediaService>();
        }
    }
}
