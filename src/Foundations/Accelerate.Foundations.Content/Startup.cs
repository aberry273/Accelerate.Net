using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Foundations.Content.Models;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Content.Services;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.EventPipelines.Services;
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
            
            services.AddDbContext<BaseContext<ContentPostLabelEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostParentEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostChannelEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostLinkEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostSettingsEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostSettingsPostEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostMentionEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<ContentPostTaxonomyEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            // Core
            services.AddTransient<IEntityService<ContentChannelEntity>, EntityService<ContentChannelEntity>>();
            services.AddTransient<IEntityService<ContentPostEntity>, EntityService<ContentPostEntity>>();
            services.AddTransient<IEntityService<ContentPostActivityEntity>, EntityService<ContentPostActivityEntity>>();
            services.AddTransient<IEntityService<ContentPostActionsEntity>, EntityService<ContentPostActionsEntity>>();
            services.AddTransient<IEntityService<ContentPostQuoteEntity>, EntityService<ContentPostQuoteEntity>>();
            services.AddTransient<IEntityService<ContentPostMediaEntity>, EntityService<ContentPostMediaEntity>>();
            services.AddTransient<IEntityService<ContentPostActionsSummaryEntity>, EntityService<ContentPostActionsSummaryEntity>>();

            services.AddTransient<IEntityService<ContentPostLabelEntity>, EntityService<ContentPostLabelEntity>>();
            services.AddTransient<IEntityService<ContentPostParentEntity>, EntityService<ContentPostParentEntity>>();
            services.AddTransient<IEntityService<ContentPostChannelEntity>, EntityService<ContentPostChannelEntity>>();
            services.AddTransient<IEntityService<ContentPostLinkEntity>, EntityService<ContentPostLinkEntity>>();
            services.AddTransient<IEntityService<ContentPostSettingsEntity>, EntityService<ContentPostSettingsEntity>>();
            services.AddTransient<IEntityService<ContentPostSettingsPostEntity>, EntityService<ContentPostSettingsPostEntity>>();
            services.AddTransient<IEntityService<ContentPostTaxonomyEntity>, EntityService<ContentPostTaxonomyEntity>>();
            services.AddTransient<IEntityService<ContentPostMentionEntity>, EntityService<ContentPostMentionEntity>>();

            // Pipeline Services
            services.AddTransient<IEntityPipelineService<ContentPostActionsEntity, IContentActionsBus>, EntityPipelineService<ContentPostActionsEntity, IContentActionsBus>>();
            services.AddTransient<IEntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus>, EntityPipelineService<ContentPostActivityEntity, IContentPostActivityBus>>();
            services.AddTransient<IEntityPipelineService<ContentPostMentionEntity, IContentPostMentionBus>, EntityPipelineService<ContentPostMentionEntity, IContentPostMentionBus>>();
            services.AddTransient<IEntityPipelineService<ContentPostQuoteEntity, IContentPostQuoteBus>, EntityPipelineService<ContentPostQuoteEntity, IContentPostQuoteBus>>();
            services.AddTransient<IEntityPipelineService<ContentPostParentEntity, IContentPostParentBus>, EntityPipelineService<ContentPostParentEntity, IContentPostParentBus>>();
            services.AddTransient<IEntityPipelineService<ContentPostEntity, IContentPostBus>, EntityPipelineService <ContentPostEntity, IContentPostBus>>();

            // Logic
            services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<ContentDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            services.AddTransient<IContentPostElasticService, ContentPostElasticService>();
            services.AddTransient<IContentActivityElasticService, ContentActivityElasticService>();
            services.AddTransient<IElasticService<ContentPostDocument>, ContentPostElasticService>();
            services.AddTransient<IElasticService<ContentPostActionsDocument>, ContentPostActionsElasticService>();
            services.AddTransient<IElasticService<ContentPostActivityDocument>, ContentActivityElasticService>();
            services.AddTransient<IElasticService<ContentChannelDocument>, ContentChannelElasticService>();
            services.AddTransient<IElasticService<ContentPostMediaDocument>, ContentPostMediaElasticService>();
            services.AddTransient<IElasticService<ContentPostActionsSummaryDocument>, ContentPostActionsSummaryElasticService>();
            services.AddTransient<IElasticService<ContentPostQuoteDocument>, ContentPostQuoteElasticService>();
            services.AddTransient<IElasticService<ContentPostLabelDocument>, ContentPostLabelElasticService>();
            //todo: migrate all others to this
            services.AddTransient<IElasticService<ContentPostSettingsDocument>, ContentPostSettingsElasticService>();
            
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
