
using Accelerate.Foundations.Transfers.Database;
using Accelerate.Foundations.Transfers.Models;
using Accelerate.Foundations.Transfers.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Transfers
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<TransfersConfiguration>(options =>
            {
                //configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<TransfersCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<TransfersPayinEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<TransfersPayoutEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<TransfersCustomerEntity>, EntityService<TransfersCustomerEntity>>();
            services.AddTransient<IEntityService<TransfersPayinEntity>, EntityService<TransfersPayinEntity>>();
            services.AddTransient<IEntityService<TransfersPayoutEntity>, EntityService<TransfersPayoutEntity>>();

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<TransfersDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<TransfersCustomerEntity> context)
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
        public static void InitializePipeline(BaseContext<TransfersPayinEntity> context)
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
        public static void InitializePipeline(BaseContext<TransfersPayoutEntity> context)
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
