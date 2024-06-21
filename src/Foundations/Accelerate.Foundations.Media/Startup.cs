
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Media.Models.Data;
using Accelerate.Foundations.Media.Models.Entities;
using Accelerate.Foundations.Media.Services;
using Accelerator.Foundation.Media.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accelerate.Foundations.Media
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            services.Configure<MediaConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            //Context
            services.AddDbContext<BaseContext<MediaBlobEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            //Services
            services.AddTransient<IEntityService<MediaBlobEntity>, EntityService<MediaBlobEntity>>();
           //Parent context for mappings
            services.AddDbContext<MediaDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            // SERVICES
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IElasticService<MediaBlobDocument>, MediaBlobElasticService>();

        }
    }
}
