
using Accelerate.Foundations.Content.EventBus;
using Accelerate.Features.Content.Hubs;
using Accelerate.Features.Content.Pipelines.Activities;
using Accelerate.Features.Content.Pipelines.Channels;
using Accelerate.Features.Content.Pipelines.Posts;
using Accelerate.Features.Content.Pipelines.Quotes;
using Accelerate.Features.Content.Pipelines.Actions;
using Accelerate.Features.Content.Services;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Content.Models.Data;
using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Pipelines;
using Accelerate.Foundations.Integrations.Elastic.Services;
using Accelerate.Foundations.Websockets.Hubs;
using Elastic.Clients.Elasticsearch.Ml;
using MassTransit;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Features.Content.Pipelines.ActionsSummary;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Features.Content.Pipelines.Mentions;
using Accelerate.Features.Content.Pipelines.Parents;
using Accelerate.Features.Content.Pipelines.Chats;
using Accelerate.Features.Content.Pipelines.Labels;
using Accelerate.Foundations.Content.Models.View;
using Accelerate.Features.Content.Pipelines.Lists;
using Accelerate.Features.Content.Pipelines.Feeds;
using Microsoft.AspNetCore.Identity;
using Accelerate.Foundations.Account.Services;
using Accelerate.Foundations.EventPipelines.Services;
using System.Threading.Channels;
using Accelerate.Foundations.Content.Hydrators;
using ImageMagick;

namespace Accelerate.Features.Content
{
    public static class Startup
    {
        /*
        private static Dictionary<Type, Type> _transits = new Dictionary<Type, Type>();
        public static void AddBus<TBus, TEntity>(Type bus, Type entity) where TBus: IBus where TEntity : IBaseEntity
        {
            _transits.Add(bus, entity);
        }
        */
        public static async Task ContentGlobalFeeds(IServiceScope scope)
        { 
            //Resolve ASP .NET Core Identity with DI help
            var service = (IEntityPipelineService<ContentFeedEntity, IContentFeedBus>)scope.ServiceProvider.GetService(typeof(IEntityPipelineService<ContentFeedEntity, IContentFeedBus>));
            var elasticService = (IElasticService<ContentFeedDocument>)scope.ServiceProvider.GetService(typeof(IElasticService<ContentFeedDocument>));
            var response = await elasticService.GetDocument<ContentFeedDocument>(Foundations.Common.Constants.Global.FeedAllGuid.ToString());
            var document = response.Source;
            if (document == null)
            {
                document = new ContentFeedDocument();
            }
            var entity = service.Get(Foundations.Common.Constants.Global.FeedAllGuid);
            if (entity == null)
            {
                var feed = new ContentFeedEntity()
                {
                    Name = "All",
                    Category = "All",
                    Id = Foundations.Common.Constants.Global.FeedAllGuid,
                    UserId = Foundations.Common.Constants.Global.GlobalAdminContent
                };

                await service.Create(feed);
                entity = feed;
            }
            entity.Hydrate(document);
            await elasticService.Index(document);
        }
        public static async Task ContentGlobalChannels(IServiceScope scope)
        {
            //Resolve ASP .NET Core Identity with DI help
            var service = (IEntityPipelineService<ContentChannelEntity, IContentChannelBus>)scope.ServiceProvider.GetService(typeof(IEntityPipelineService<ContentChannelEntity, IContentChannelBus>));
            var elasticService = (IElasticService<ContentChannelDocument>)scope.ServiceProvider.GetService(typeof(IElasticService<ContentChannelDocument>));
            var response = await elasticService.GetDocument<ContentChannelDocument>(Foundations.Common.Constants.Global.ChannelNewsGuid.ToString());
            var document = response.Source;
            if (document == null)
            {
                document = new ContentChannelDocument();
            }
            var entity = service.Get(Foundations.Common.Constants.Global.ChannelNewsGuid);
            if (entity == null)
            {
                var channel = new ContentChannelEntity()
                {
                    Name = "News",
                    Category = "News",
                    Id = Foundations.Common.Constants.Global.ChannelNewsGuid,
                    UserId = Foundations.Common.Constants.Global.GlobalAdminContent
                };
                await service.Create(channel);
                entity = channel;
            }
            entity.Hydrate(document);
            await elasticService.Index(document);
        }
        public static void ConfigureApp(WebApplication app)
        { 
            app.MapHub<BaseHub<ContentPostViewDocument>>($"/{Constants.Settings.ContentPostsHubName}");
            app.MapHub<BaseHub<ContentPostActionsDocument>>($"/{Constants.Settings.ContentPostActionsHubName}");
            app.MapHub<BaseHub<ContentChannelDocument>>($"/{Constants.Settings.ContentChannelsHubName}");
            app.MapHub<BaseHub<ContentFeedDocument>>($"/{Constants.Settings.ContentFeedsHubName}");
            app.MapHub<BaseHub<ContentChatDocument>>($"/{Constants.Settings.ContentChatsHubName}");
            app.MapHub<BaseHub<ContentListDocument>>($"/{Constants.Settings.ContentListsHubName}");
            app.MapHub<BaseHub<ContentPostActionsSummaryDocument>>($"/{Constants.Settings.ContentPostActionsSummaryHubName}");
            app.MapHub<BaseHub<ContentPostSettingsDocument>>($"/{Constants.Settings.ContentPostSettingsHubName}");
            app.MapHub<BaseHub<ContentPostActivityDocument>>($"/{Constants.Settings.ContentPostActivitiesHubName}");

            using (var scope = app.Services.CreateScope())
            {
                var task1 = Task.Run(async () => await ContentGlobalChannels(scope));
                task1.Wait();
                var task2 = Task.Run(async () => await ContentGlobalFeeds(scope));
                task2.Wait();
            }
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            // Redundant
            services.AddTransient<IContentThreadViewService, ContentThreadViewService>();
            // In Use
            services.AddTransient<IContentViewSearchService, ContentViewSearchService>();
            services.AddTransient<IBaseContentEntityViewService<ContentChannelDocument>, ContentChannelEntityViewService>();
            services.AddTransient<IBaseContentEntityViewService<ContentListDocument>, ContentListEntityViewService>();
            services.AddTransient<IBaseContentEntityViewService<ContentFeedDocument>, ContentFeedEntityViewService>();
            services.AddTransient<IBaseContentEntityViewService<ContentChatDocument>, ContentChatEntityViewService>();
            services.AddTransient<IBaseContentEntityViewService<ContentPostDocument>, ContentPostEntityViewService>();

            services.AddTransient<BaseHub<ContentPostViewDocument>, ContentPostHub>();
            services.AddTransient<BaseHub<ContentPostActionsDocument>, ContentPostActionsHub>();
            services.AddTransient<BaseHub<ContentPostActionsSummaryDocument>, ContentPostActionsSummaryHub>();
            services.AddTransient<BaseHub<ContentChannelDocument>, ContentChannelHub>();
            services.AddTransient<BaseHub<ContentListDocument>, ContentListHub>();
            services.AddTransient<BaseHub<ContentFeedDocument>, ContentFeedHub>();
            services.AddTransient<BaseHub<ContentChatDocument>, ContentChatHub>();
            services.AddTransient<BaseHub<ContentPostActivityDocument>, ContentPostActivityHub>();
            services.AddTransient<BaseHub<ContentPostQuoteDocument>, ContentPostQuoteHub>();
            services.AddTransient<BaseHub<ContentPostSettingsDocument>, ContentPostSettingsHub>();

            // Posts
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostEntity, ContentPostCreatedPipeline, ContentPostUpdatedPipeline, ContentPostDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostEntity>(services);
            //Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostEntity, ContentPostCreatedCompletedPipeline, ContentPostUpdatedCompletedPipeline, ContentPostDeletedCompletedPipeline>(services);
         
            // Parent Posts
            // Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostParentEntity, ContentPostCreatedListenerPipeline, EmptyUpdatedPipeline<ContentPostParentEntity>, EmptyDeletedPipeline<ContentPostParentEntity>>(services);
            // Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostParentEntity>(services);
            // Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostParentEntity, IContentPostParentBus>(services);

            // Actions
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedPipeline, ContentPostActionUpdatedPipeline, ContentPostActionsDeletedPipeline>(services);
           // Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostActionsEntity>(services);
             
            // other

            // Parents
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostParentEntity, ContentPostParentCreatedPipeline, EmptyUpdatedPipeline<ContentPostParentEntity>, EmptyDeletedPipeline<ContentPostParentEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostParentEntity>(services);

            // ActionSummary
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActionsSummaryEntity, ContentPostActionsSummaryCreatedPipeline, ContentPostActionsSummaryUpdatedPipeline, ContentPostActionsSummaryDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostActionsSummaryEntity>(services);
            //Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsSummaryEntity, IContentActionsSummaryBus>(services);

            // Activities // TODO CHANGE TO NOTIFICATIONS
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActivityEntity, ContentPostActivityCreatedPipeline, ContentPostActivityUpdatedPipeline, ContentPostActivityDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostActivityEntity>(services); 
            
            // Activities completed
            //Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostActivityEntity, ContentPostActionsCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostActivityEntity>, EmptyDeletedCompletedPipeline<ContentPostActivityEntity>>(services);

            // Channels
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentChannelEntity, ContentChannelCreatedPipeline, ContentChannelUpdatedPipeline, ContentChannelDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentChannelEntity, ContentChannelCreateCompletedPipeline, ContentChannelUpdatedCompletedPipeline, ContentChannelDeleteCompletedPipeline>(services);

            // Chats
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentChatEntity, ContentChatCreatedPipeline, ContentChatUpdatedPipeline, ContentChatDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentChatEntity>(services);

            // Feeds
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentFeedEntity, ContentFeedCreatedPipeline, ContentFeedUpdatedPipeline, ContentFeedDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentFeedEntity>(services);

            // Lists
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentListEntity, ContentListCreatedPipeline, ContentListUpdatedPipeline, ContentListDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentListEntity>(services);

            // Quotes
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostQuoteEntity, ContentPostQuoteCreatedPipeline, ContentPostQuoteUpdatedPipeline, ContentPostQuoteDeletedPipeline>(services);
            //Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostQuoteEntity>(services);

            // Mentions
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostMentionEntity, ContentPostMentionCreatedPipeline, EmptyUpdatedPipeline<ContentPostMentionEntity>, EmptyDeletedPipeline<ContentPostMentionEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostMentionEntity>(services);
            // Mentions
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostLabelEntity, ContentPostLabelCreatedPipeline, EmptyUpdatedPipeline<ContentPostLabelEntity>, EmptyDeletedPipeline<ContentPostLabelEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostLabelEntity>(services);
            // Pins
            Foundations.EventPipelines.Startup.ConfigureEmptyPipelineServices<ContentPostPinEntity>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostPinEntity>(services);

            // Settings

            // Listener pipelines
            // ActionSummary
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedCompletedListenerPipeline, ContentPostActionsUpdatedCompletedListenerPipeline, EmptyDeletedCompletedPipeline<ContentPostActionsEntity>>(services);
            //ContentPosts
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostParentEntity, ContentPostParentsCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostParentEntity>, EmptyDeletedCompletedPipeline<ContentPostParentEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostQuoteEntity, ContentPostQuotesCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostQuoteEntity>, EmptyDeletedCompletedPipeline<ContentPostQuoteEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostLabelEntity, ContentPostLabelCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostLabelEntity>, EmptyDeletedCompletedPipeline<ContentPostLabelEntity>>(services);

            // MassTransit Busses
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostEntity, IContentPostBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsEntity, IContentActionsBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsSummaryEntity, IContentActionsSummaryBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostQuoteEntity, IContentPostQuoteBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostMentionEntity, IContentPostMentionBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostParentEntity, IContentPostParentBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActivityEntity, IContentPostActivityBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostLabelEntity, IContentPostLabelBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostPinEntity, IContentPostPinBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentChannelEntity, IContentChannelBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentListEntity, IContentListBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentFeedEntity, IContentFeedBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentChatEntity, IContentChatBus>(services);

        }
    }
}
