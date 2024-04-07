using Accelerate.Features.Content.Consumers;
using Accelerate.Features.Content.EventBus;
using Accelerate.Features.Content.Hubs;
using Accelerate.Features.Content.Pipelines.Channels;
using Accelerate.Features.Content.Pipelines.Posts;
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
using MassTransit;
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
            app.MapHub<BaseHub<ContentChannelDocument>>($"/{Constants.Settings.ContentChannelsHubName}");
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES
            services.AddTransient<IContentViewService, ContentViewService>();

            services.AddTransient<BaseHub<ContentPostDocument>, ContentPostHub>();
            services.AddTransient<BaseHub<ContentPostReviewDocument>, ContentPostReviewHub>();
            services.AddTransient<BaseHub<ContentChannelDocument>, ContentChannelHub>();
            //AddBus<IContentPostBus, ContentPostEntity>(typeof(IContentPostBus), typeof(ContentPostEntity));// (IContentBus, ContentPostEntity);

            /*
            var assembly = Assembly.GetExecutingAssembly(); // Adjust as needed
            services.RegisterServicesWithAttributes(assembly);

            */
            // POSTS

            services.AddTransient<IDataCreateEventPipeline<ContentPostEntity>, ContentPostCreatedPipeline>();
            services.AddTransient<IDataCreateCompletedEventPipeline<ContentPostEntity>, ContentPostCreateCompletedPipeline>();
            services.AddTransient<IDataUpdateEventPipeline<ContentPostEntity>, ContentPostUpdatedPipeline>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<ContentPostEntity>, ContentPostUpdateCompletedPipeline>();
            services.AddTransient<IDataDeleteEventPipeline<ContentPostEntity>, ContentPostDeletedPipeline>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<ContentPostEntity>, ContentPostDeleteCompletedPipeline>();
              
            services.AddMassTransit<IContentPostBus>(x =>
            {
                // Posts
                x.AddConsumer<DataCreateConsumer<ContentPostEntity, IContentPostBus>>();
                x.AddConsumer<DataCreateCompleteConsumer<ContentPostEntity>>();
                x.AddConsumer<DataUpdateConsumer<ContentPostEntity, IContentPostBus>>();
                x.AddConsumer<DataUpdateCompleteConsumer<ContentPostEntity>>();
                x.AddConsumer<DataDeleteConsumer<ContentPostEntity, IContentPostBus>>();
                x.AddConsumer<DataDeleteCompleteConsumer<ContentPostEntity>>();
                // Entities


                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<ContentPostEntity, IContentPostBus>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<ContentPostEntity>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<ContentPostEntity, IContentPostBus>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<ContentPostEntity>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<ContentPostEntity, IContentPostBus>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<ContentPostEntity>>(context);
                    });
                });
            });
            
            // Reviews
            services.AddTransient<IDataCreateEventPipeline<ContentPostReviewEntity>, ContentPostReviewCreatedPipeline>();
            services.AddTransient<IDataCreateCompletedEventPipeline<ContentPostReviewEntity>, EmptyCreatedCompletedPipeline<ContentPostReviewEntity>>();
            services.AddTransient<IDataUpdateEventPipeline<ContentPostReviewEntity>, ContentPostReviewUpdatedPipeline>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<ContentPostReviewEntity>, EmptyUpdatedCompletedPipeline<ContentPostReviewEntity>>();
            services.AddTransient<IDataDeleteEventPipeline<ContentPostReviewEntity>, EmptyDeletedPipeline<ContentPostReviewEntity>>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<ContentPostReviewEntity>, EmptyDeletedCompletedPipeline<ContentPostReviewEntity>>();
            
            services.AddMassTransit<IContentReviewBus>(x =>
            {
                // Posts
                x.AddConsumer<DataCreateConsumer<ContentPostReviewEntity, IContentReviewBus>>();
                x.AddConsumer<DataCreateCompleteConsumer<ContentPostReviewEntity>>();
                x.AddConsumer<DataUpdateConsumer<ContentPostReviewEntity, IContentReviewBus>>();
                x.AddConsumer<DataUpdateCompleteConsumer<ContentPostReviewEntity>>();
                x.AddConsumer<DataDeleteConsumer<ContentPostReviewEntity, IContentReviewBus>>();
                x.AddConsumer<DataDeleteCompleteConsumer<ContentPostReviewEntity>>();
                // Entities


                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<ContentPostReviewEntity, IContentReviewBus>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<ContentPostReviewEntity>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<ContentPostReviewEntity, IContentReviewBus>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<ContentPostReviewEntity>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<ContentPostReviewEntity, IContentReviewBus>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<ContentPostReviewEntity>>(context);
                    });
                });
            });


            // Channels
            services.AddTransient<IDataCreateEventPipeline<ContentChannelEntity>, ContentChannelCreatedPipeline>();
            services.AddTransient<IDataCreateCompletedEventPipeline<ContentChannelEntity>, ContentChannelCreateCompletedPipeline>();
            services.AddTransient<IDataUpdateEventPipeline<ContentChannelEntity>, ContentChannelUpdatedPipeline>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<ContentChannelEntity>, ContentChannelUpdatedCompletedPipeline>();
            services.AddTransient<IDataDeleteEventPipeline<ContentChannelEntity>, ContentChannelDeletedPipeline>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<ContentChannelEntity>, ContentChannelDeleteCompletedPipeline>();

            services.AddMassTransit<IContentChannelBus>(x =>
            {
                // Posts
                x.AddConsumer<DataCreateConsumer<ContentChannelEntity, IContentChannelBus>>();
                x.AddConsumer<DataCreateCompleteConsumer<ContentChannelEntity>>();
                x.AddConsumer<DataUpdateConsumer<ContentChannelEntity, IContentChannelBus>>();
                x.AddConsumer<DataUpdateCompleteConsumer<ContentChannelEntity>>();
                x.AddConsumer<DataDeleteConsumer<ContentChannelEntity, IContentChannelBus>>();
                x.AddConsumer<DataDeleteCompleteConsumer<ContentChannelEntity>>();
                // Entities


                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<ContentChannelEntity, IContentChannelBus>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<ContentChannelEntity>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<ContentChannelEntity, IContentChannelBus>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<ContentChannelEntity>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<ContentChannelEntity, IContentChannelBus>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<ContentChannelEntity>>(context);
                    });
                });
            });
        }
    }
}
