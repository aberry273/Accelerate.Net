
using Accelerate.Foundations.Settlements.Database;
using Accelerate.Foundations.Settlements.Models;
using Accelerate.Foundations.Settlements.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Mediator.Behaviours;
using Azure.Identity;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using static Accelerate.Foundations.Database.Constants.Exceptions;

namespace Accelerate.Foundations.Settlements
{
    public static class Startup
    {
        public static void ConfigureCommands(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            ///Mass transit
            //services.AddMediator(x => x.AddConsumersFromNamespaceContaining<SettlementsAddressEntity>());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<SettlementsConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            // Core contexts
            services.AddDbContext<BaseContext<SettlementsLedgerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<SettlementsLedgerStatementEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<SettlementsLedgerTransactionEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            // Junction contexts
            
            //Services
            services.AddTransient<IEntityService<SettlementsLedgerEntity>, EntityService<SettlementsLedgerEntity>>();
            services.AddTransient<IEntityService<SettlementsLedgerStatementEntity>, EntityService<SettlementsLedgerStatementEntity>>();
            services.AddTransient<IEntityService<SettlementsLedgerTransactionEntity>, EntityService<SettlementsLedgerTransactionEntity>>();

            //CQRS
            ConfigureCommands(services, configuration);

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<SettlementsDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<SettlementsLedgerEntity> context)
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
        public static void InitializePipeline(BaseContext<SettlementsLedgerStatementEntity> context)
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
        public static void InitializePipeline(BaseContext<SettlementsLedgerTransactionEntity> context)
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
