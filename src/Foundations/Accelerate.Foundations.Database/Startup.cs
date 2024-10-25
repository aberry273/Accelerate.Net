using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelerate.Foundations.Database
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }
        public static void InitializePipeline()
        {
            try
            {
                //Load

                //Config/Pipelines.xml

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
