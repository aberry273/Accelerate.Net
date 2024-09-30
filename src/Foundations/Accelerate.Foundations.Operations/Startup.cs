using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Operations.Models.Data;
using Accelerate.Foundations.Database.Services;
using Microsoft.EntityFrameworkCore;
using Accelerate.Foundations.Operations.Models.Entities;
using Accelerator.Foundation.Content.Database;

namespace Accelerate.Foundations.Operations
{

    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<OperationsConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<OperationsJobEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<OperationsActionEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<OperationsActivityEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<OperationsJobEntity>, EntityService<OperationsJobEntity>>();
            services.AddTransient<IEntityService<OperationsActionEntity>, EntityService<OperationsActionEntity>>();
            services.AddTransient<IEntityService<OperationsActivityEntity>, EntityService<OperationsActivityEntity>>();

            //Parent context for mappings
            services.AddDbContext<OperationsDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
        }
        public static void InitializePipeline(BaseContext<OperationsJobEntity> context)
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}