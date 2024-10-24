
using Accelerate.Foundations.Accounts.Database;
using Accelerate.Foundations.Accounts.Models;
using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Accounts
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<AccountsConfiguration>(options =>
            {
                //configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<AccountsCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsBankAccountEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
           
            //Services
            // Core
            services.AddTransient<IEntityService<AccountsCustomerEntity>, EntityService<AccountsCustomerEntity>>();
            services.AddTransient<IEntityService<AccountsBankAccountEntity>, EntityService<AccountsBankAccountEntity>>();
           
            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<AccountsDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<AccountsCustomerEntity> context)
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
        public static void InitializePipeline(BaseContext<AccountsBankAccountEntity> context)
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
