
using Accelerate.Foundations.Transactions.Database;
using Accelerate.Foundations.Transactions.Models;
using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Transactions
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<TransactionsConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context

            services.AddDbContext<BaseContext<TransactionsCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<TransactionsTransactionEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<TransactionsAddressEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            //services.AddDbContext<BaseContext<TransactionsWalletEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<TransactionsCustomerEntity>, EntityService<TransactionsCustomerEntity>>();
            services.AddTransient<IEntityService<TransactionsTransactionEntity>, EntityService<TransactionsTransactionEntity>>();
            services.AddTransient<IEntityService<TransactionsAddressEntity>, EntityService<TransactionsAddressEntity>>();
            //services.AddTransient<IEntityService<TransactionsWalletEntity>, EntityService<TransactionsWalletEntity>>();

            // Logic

            //Parent context for mappings
            services.AddDbContext<TransactionsDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<TransactionsCustomerEntity> context)
        {
            try {
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static void InitializePipeline(BaseContext<TransactionsTransactionEntity> context)
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
