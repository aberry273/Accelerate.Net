
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Hubs;
using Accelerate.Features.Content.Pipelines.Activities;
using Accelerate.Features.Content.Pipelines.Channels;
using Accelerate.Features.Content.Pipelines.Posts;
using Accelerate.Features.Content.Pipelines.Quotes;
using Accelerate.Features.Content.Pipelines.Reviews;
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
            app.MapHub<BaseHub<ContentPostReviewDocument>>($"/{Constants.Settings.ContentPostReviewsHubName}");
            app.MapHub<BaseHub<ContentPostQuoteDocument>>($"/{Constants.Settings.ContentChannelsHubName}");
            app.MapHub<BaseHub<ContentChannelDocument>>($"/{Constants.Settings.ContentChannelsHubName}");

        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddTransient<IContentViewService, ContentViewService>();

            services.AddTransient<BaseHub<ContentPostDocument>, ContentPostHub>();
            services.AddTransient<BaseHub<ContentPostReviewDocument>, ContentPostReviewHub>();
            services.AddTransient<BaseHub<ContentChannelDocument>, ContentChannelHub>();
            services.AddTransient<BaseHub<ContentPostActivityDocument>, ContentPostActivityHub>();
            services.AddTransient<BaseHub<ContentPostQuoteDocument>, ContentPostQuoteHub>();

            // POSTS
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostEntity, ContentPostCreatedPipeline, ContentPostUpdatedPipeline, ContentPostDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureCompletedPipelineServices<ContentPostEntity, ContentPostCreateCompletedPipeline, ContentPostUpdateCompletedPipeline, ContentPostDeleteCompletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostEntity, IContentPostBus>(services);

            // Reviews
            Foundations.EventPipelines.Startup.ConfigurePipelineServices<ContentPostReviewEntity, ContentPostReviewCreatedPipeline, ContentPostReviewUpdatedPipeline, ContentPostReviewDeletedPipeline>(services);
            Foundations.EventPipelines.Startup.ConfigureEmptyCompletedPipelineServices<ContentPostReviewEntity>(services);
            Foundations.EventPipelines.Startup.ConfigureMassTransitServices<ContentPostReviewEntity, IContentReviewBus>(services);

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

        }
    }
}
