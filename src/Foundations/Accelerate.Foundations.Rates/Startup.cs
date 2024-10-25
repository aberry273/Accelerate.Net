
using Accelerate.Foundations.Rates.Database;
using Accelerate.Foundations.Rates.Models;
using Accelerate.Foundations.Rates.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Rates
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<RatesConfiguration>(options =>
            {
                //configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<RatesCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<RatesConversionQuoteEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<RatesConversionOrderEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<RatesCustomerEntity>, EntityService<RatesCustomerEntity>>();
            services.AddTransient<IEntityService<RatesConversionQuoteEntity>, EntityService<RatesConversionQuoteEntity>>();
            services.AddTransient<IEntityService<RatesConversionOrderEntity>, EntityService<RatesConversionOrderEntity>>();

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<RatesDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<RatesCustomerEntity> context)
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
        public static void InitializePipeline(BaseContext<RatesConversionQuoteEntity> context)
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
        public static void InitializePipeline(BaseContext<RatesConversionOrderEntity> context)
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
