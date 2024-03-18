using Accelerate.Foundations.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
namespace Accelerate.Foundations.Common
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMetaContentService, MetaContentService>();
        }
    }
}
