using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerator.Foundation.Content.Database;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Content
{
    public static class Startup
    { 
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<ContentConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            //Context
            services.AddDbContext<BaseContext<ContentChannelEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostActivityEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostActionsEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostActionsSummaryEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostQuoteEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostMediaEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            //Services
            services.AddTransient<IEntityService<ContentChannelEntity>, EntityService<ContentChannelEntity>>();
            services.AddTransient<IEntityService<ContentPostEntity>, EntityService<ContentPostEntity>>();
            services.AddTransient<IEntityService<ContentPostActivityEntity>, EntityService<ContentPostActivityEntity>>();
            services.AddTransient<IEntityService<ContentPostActionsEntity>, EntityService<ContentPostActionsEntity>>();
            services.AddTransient<IEntityService<ContentPostQuoteEntity>, EntityService<ContentPostQuoteEntity>>();
            services.AddTransient<IEntityService<ContentPostMediaEntity>, EntityService<ContentPostMediaEntity>>();
            services.AddTransient<IEntityService<ContentPostActionsSummaryEntity>, EntityService<ContentPostActionsSummaryEntity>>();
            //Parent context for mappings
            services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            services.AddTransient<IContentPostElasticService, ContentPostElasticService>();
            services.AddTransient<IElasticService<ContentPostDocument>, ContentPostElasticService>();
            services.AddTransient<IElasticService<ContentPostActionsDocument>, ContentPostActionsElasticService>();
            services.AddTransient<IElasticService<ContentPostActivityDocument>, ContentActivityElasticService>();
            services.AddTransient<IElasticService<ContentChannelDocument>, ContentChannelElasticService>();
            services.AddTransient<IElasticService<ContentPostMediaDocument>, ContentPostMediaElasticService>();
            services.AddTransient<IElasticService<ContentPostActionsSummaryDocument>, ContentPostActionsSummaryElasticService>();

            services.AddTransient<IElasticService<ContentPostQuoteDocument>, ContentPostQuoteElasticService>();
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
