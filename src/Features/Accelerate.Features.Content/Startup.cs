
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
using Twilio.TwiML.Voice;
using static Accelerate.Foundations.Database.Constants.Exceptions;
using Accelerate.Features.Content.Pipelines.ActionsSummary;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Features.Content.Pipelines.Mentions;
using Accelerate.Features.Content.Pipelines.Parents;

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
        public static void ConfigureApp(WebApplication app)
        {
            app.MapHub<BaseHub<ContentPostDocument>>($"/{Constants.Settings.ContentPostsHubName}");
            app.MapHub<BaseHub<ContentPostActionsDocument>>($"/{Constants.Settings.ContentPostActionsHubName}");
            app.MapHub<BaseHub<ContentChannelDocument>>($"/{Constants.Settings.ContentChannelsHubName}");
            app.MapHub<BaseHub<ContentPostActionsSummaryDocument>>($"/{Constants.Settings.ContentPostActionsSummaryHubName}");
            app.MapHub<BaseHub<ContentPostSettingsDocument>>($"/{Constants.Settings.ContentPostSettingsHubName}");
            app.MapHub<BaseHub<ContentPostActivityDocument>>($"/{Constants.Settings.ContentPostActivitiesHubName}");


        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddTransient<IContentViewService, ContentViewService>();

            services.AddTransient<BaseHub<ContentPostDocument>, ContentPostHub>();
            services.AddTransient<BaseHub<ContentPostActionsDocument>, ContentPostActionsHub>();
            services.AddTransient<BaseHub<ContentPostActionsSummaryDocument>, ContentPostActionsSummaryHub>();
            services.AddTransient<BaseHub<ContentChannelDocument>, ContentChannelHub>();
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
            
            // Quotes
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostQuoteEntity, ContentPostQuoteCreatedPipeline, ContentPostQuoteUpdatedPipeline, ContentPostQuoteDeletedPipeline>(services);
            //Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostQuoteEntity>(services);

            // Mentions
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostMentionEntity, ContentPostMentionCreatedPipeline, EmptyUpdatedPipeline<ContentPostMentionEntity>, EmptyDeletedPipeline<ContentPostMentionEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostMentionEntity>(services);

            // Settings

            // Listener pipelines
            // ActionSummary > Action listeners
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedCompletedListenerPipeline, ContentPostActionsUpdatedCompletedListenerPipeline, EmptyDeletedCompletedPipeline<ContentPostActionsEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostParentEntity, ContentPostParentsCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostParentEntity>, EmptyDeletedCompletedPipeline<ContentPostParentEntity>>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostQuoteEntity, ContentPostQuotesCreatedCompletedListenerPipeline, EmptyUpdatedCompletedPipeline<ContentPostQuoteEntity>, EmptyDeletedCompletedPipeline<ContentPostQuoteEntity>>(services);

            // MassTransit Busses
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostEntity, IContentPostBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsEntity, IContentActionsBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsSummaryEntity, IContentActionsSummaryBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostQuoteEntity, IContentPostQuoteBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostMentionEntity, IContentPostMentionBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostParentEntity, IContentPostParentBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActivityEntity, IContentPostActivityBus>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentChannelEntity, IContentChannelBus>(services);
        }
    }
}
