using MassTransit;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Accelerate.Foundations.Mediator.Behaviours;

namespace Accelerate.Foundations.Mediator
{
    public static class Startup
    {
        public static void ConfigureServices<T, B>(IServiceCollection services)// where B : class, IDataBus<T>
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            // To replace with
            /*
            services.AddMediator(cfg =>
            {
                cfg.AddConsumer<SubmitOrderConsumer>();
                cfg.AddConsumer<OrderStatusConsumer>();
            });
            */



            /*
            services.AddMassTransit<B>(x =>
            {
                x.AddConsumer<DataCreateConsumer<T, B>>();
                x.AddConsumer<DataCreateCompleteConsumer<T>>();
                x.AddConsumer<DataUpdateConsumer<T, B>>();
                x.AddConsumer<DataUpdateCompleteConsumer<T>>();
                x.AddConsumer<DataDeleteConsumer<T, B>>();
                x.AddConsumer<DataDeleteCompleteConsumer<T>>();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("event-listener", e =>
                    {
                        // Content Posts
                        e.ConfigureConsumers(context);
                      
                    });
                });
            });
            */
        }
        /*
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
        */
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // SERVICES 
        }
    }
}
