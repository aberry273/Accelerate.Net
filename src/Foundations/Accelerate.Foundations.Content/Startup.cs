using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerator.Foundation.Content.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Content
{
    public static class Startup
    { 
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //get secret
            // CONFIGS
            var connString = configuration.GetConnectionString(Constants.Config.LocalDatabaseKey) ?? configuration[Constants.Config.DatabaseKey];
            //Context
            services.AddDbContext<BaseContext<ContentChannelEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostActivityEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostReviewEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostQuoteEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            //Services
            services.AddTransient<IEntityService<ContentChannelEntity>, EntityService<ContentChannelEntity>>();
            services.AddTransient<IEntityService<ContentPostEntity>, EntityService<ContentPostEntity>>();
            services.AddTransient<IEntityService<ContentPostActivityEntity>, EntityService<ContentPostActivityEntity>>();
            services.AddTransient<IEntityService<ContentPostReviewEntity>, EntityService<ContentPostReviewEntity>>();
            services.AddTransient<IEntityService<ContentPostQuoteEntity>, EntityService<ContentPostQuoteEntity>>();
            //Parent context for mappings
            services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


            services.AddTransient<IContentPostElasticService, ContentPostElasticService>();
            services.AddTransient<IElasticService<ContentPostDocument>, ContentPostElasticService>();
            services.AddTransient<IElasticService<ContentPostReviewDocument>, ContentReviewElasticService>();
            services.AddTransient<IElasticService<ContentPostActivityDocument>, ContentActivityElasticService>();
            services.AddTransient<IElasticService<ContentChannelDocument>, ContentChannelElasticService>();
            
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
