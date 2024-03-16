using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Database.Services;
using Accelerator.Foundation.Finance.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelerate.Foundations.Content
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //Context
            services.AddDbContext<BaseContext<ContentPostEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Constants.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            //Services
            services.AddTransient<IEntityService<ContentPostEntity>, EntityService<ContentPostEntity>>();
            //Parent context for mappings
            services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(Constants.Settings.ConnectionStringName)), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<ContentPostEntity> context)
        {
            try
            {
                //context.Database.EnsureCreated();
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
