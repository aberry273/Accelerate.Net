
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

            // POSTS
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostEntity, ContentPostCreatedPipeline, ContentPostUpdatedPipeline, ContentPostDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostEntity, ContentPostCreatedCompletedPipeline, ContentPostUpdatedCompletedPipeline, ContentPostDeletedCompletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostEntity, IContentPostBus>(services);

            // Actions
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedPipeline, ContentPostActionUpdatedPipeline, ContentPostActionsDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedCompletedPipeline, ContentPostActionsUpdatedCompletedPipeline, ContentPostActionsDeletedCompletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsEntity, IContentActionsBus>(services);
            // other
           
            // ActionSummary
            //Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActionsSummaryEntity, ContentPostActionsSummaryCreatedPipeline, ContentPostActionsSummaryUpdatedPipeline, ContentPostActionsSummaryDeletedPipeline>(services);
            //Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostActionsSummaryEntity>(services);
            //Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActionsSummaryEntity, IContentActionsSummaryBus>(services);
            // ActionSummary > Action listeners
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActionsEntity, ContentPostActionsCreatedListenerPipeline, ContentPostActionsUpdatedListenerPipeline, EmptyDeletedPipeline<ContentPostActionsEntity>>(services);

            // Activities
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostActivityEntity, ContentPostActivityCreatedPipeline, ContentPostActivityUpdatedPipeline, ContentPostActivityDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostActivityEntity>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostActivityEntity, IContentPostActivityBus>(services);

            // Channels
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentChannelEntity, ContentChannelCreatedPipeline, ContentChannelUpdatedPipeline, ContentChannelDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentChannelEntity, ContentChannelCreateCompletedPipeline, ContentChannelUpdatedCompletedPipeline, ContentChannelDeleteCompletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentChannelEntity, IContentChannelBus>(services);
            
            // Quotes
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostQuoteEntity, ContentPostQuoteCreatedPipeline, ContentPostQuoteUpdatedPipeline, ContentPostQuoteDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostQuoteEntity>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostQuoteEntity, IContentPostQuoteBus>(services);

            // Settings
        }
    }
}
