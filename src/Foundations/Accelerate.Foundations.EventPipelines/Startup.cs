using Accelerate.Foundations.Database.Models;
using Accelerate.Foundations.EventPipelines.Consumers;
using Accelerate.Foundations.EventPipelines.EventBus;
using Accelerate.Foundations.EventPipelines.Pipelines;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Accelerate.Foundations.EventPipelines
{
    public static class Startup
    {
        public static void ConfigureMassTransitServices<T, B>(IServiceCollection services) where T : IBaseEntity where B : class, IDataBus<T>
        {
            services.AddMassTransit<B>(x =>
            {
                //x.AddConsumersFromNamespaceContaining<T>();
                // Posts
                x.AddConsumer<DataCreateConsumer<T, B>>();
                x.AddConsumer<DataCreateCompleteConsumer<T>>();
                x.AddConsumer<DataUpdateConsumer<T, B>>();
                x.AddConsumer<DataUpdateCompleteConsumer<T>>();
                x.AddConsumer<DataDeleteConsumer<T, B>>();
                x.AddConsumer<DataDeleteCompleteConsumer<T>>();
                // Entities


                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumer<DataCreateConsumer<T, B>>(context);
                        e.ConfigureConsumer<DataCreateCompleteConsumer<T>>(context);

                        e.ConfigureConsumer<DataUpdateConsumer<T, B>>(context);
                        e.ConfigureConsumer<DataUpdateCompleteConsumer<T>>(context);

                        e.ConfigureConsumer<DataDeleteConsumer<T, B>>(context);
                        e.ConfigureConsumer<DataDeleteCompleteConsumer<T>>(context);
                    });
                });
            });
        }
        public static void ConfigureEmptyPipelineServices<T>(IServiceCollection services)
        {
            services.AddTransient<IDataCreateEventPipeline<T>, EmptyCreatedPipeline<T>>();
            services.AddTransient<IDataUpdateEventPipeline<T>, EmptyUpdatedPipeline<T>>();
            services.AddTransient<IDataDeleteEventPipeline<T>, EmptyDeletedPipeline<T>>();
        }
        public static void ConfigureEmptyCompletedPipelineServices<T>(IServiceCollection services)
        {
            services.AddTransient<IDataCreateCompletedEventPipeline<T>, EmptyCreatedCompletedPipeline<T>>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<T>, EmptyUpdatedCompletedPipeline<T>>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<T>, EmptyDeletedCompletedPipeline<T>>();
        }
        public static void ConfigureCompletedPipelineServices<T, C, U, D>(IServiceCollection services) where C : DataCreateCompletedEventPipeline<T> where U : DataUpdateCompletedEventPipeline<T> where D : DataDeleteCompletedEventPipeline<T>
        {
            services.AddTransient<IDataCreateCompletedEventPipeline<T>, C>();
            services.AddTransient<IDataUpdateCompletedEventPipeline<T>, U>();
            services.AddTransient<IDataDeleteCompletedEventPipeline<T>, D>();
        }
        public static void ConfigurePipelineServices<T, C, U, D>(IServiceCollection services) where C : DataCreateEventPipeline<T> where U : DataUpdateEventPipeline<T> where D : DataDeleteEventPipeline<T>
        {
            services.AddTransient<IDataCreateEventPipeline<T>, C>();
            services.AddTransient<IDataUpdateEventPipeline<T>, U>();
            services.AddTransient<IDataDeleteEventPipeline<T>, D>();
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES 
        }
    }
}
