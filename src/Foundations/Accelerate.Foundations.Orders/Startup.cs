
using Accelerate.Foundations.Orders.Database;
using Accelerate.Foundations.Orders.Models;
using Accelerate.Foundations.Orders.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Orders
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<OrdersConfiguration>(options =>
            {
                //configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<OrdersCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<OrdersPurchaseOrderEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<OrdersPurchaseQuoteEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<OrdersCustomerEntity>, EntityService<OrdersCustomerEntity>>();
            services.AddTransient<IEntityService<OrdersPurchaseOrderEntity>, EntityService<OrdersPurchaseOrderEntity>>();
            services.AddTransient<IEntityService<OrdersPurchaseQuoteEntity>, EntityService<OrdersPurchaseQuoteEntity>>();

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<OrdersDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<OrdersCustomerEntity> context)
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
        public static void InitializePipeline(BaseContext<OrdersPurchaseOrderEntity> context)
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
        public static void InitializePipeline(BaseContext<OrdersPurchaseQuoteEntity> context)
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
