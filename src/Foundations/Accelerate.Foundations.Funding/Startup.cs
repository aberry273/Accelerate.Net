
using Accelerate.Foundations.Funding.Database;
using Accelerate.Foundations.Funding.Models;
using Accelerate.Foundations.Funding.Models.Entities;
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

namespace Accelerate.Foundations.Funding
{
    public static class Startup
    {
        public static void ConfigureCommands(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            ///Mass transit
            //services.AddMediator(x => x.AddConsumersFromNamespaceContaining<FundingAddressEntity>());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<FundingConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            // Core contexts
            services.AddDbContext<BaseContext<FundingBankAccountEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FundingWalletEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FundingVirtualAccountEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            // Junction contexts
            services.AddDbContext<BaseContext<FundingSourceEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FundingCustomerEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            services.AddTransient<IEntityService<FundingBankAccountEntity>, EntityService<FundingBankAccountEntity>>();
            services.AddTransient<IEntityService<FundingWalletEntity>, EntityService<FundingWalletEntity>>();
            services.AddTransient<IEntityService<FundingVirtualAccountEntity>, EntityService<FundingVirtualAccountEntity>>();
            services.AddTransient<IEntityService<FundingSourceEntity>, EntityService<FundingSourceEntity>>(); 
            services.AddTransient<IEntityService<FundingCustomerEntity>, EntityService<FundingCustomerEntity>>();

            //CQRS
            ConfigureCommands(services, configuration);

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<FundingDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<FundingBankAccountEntity> context)
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
        public static void InitializePipeline(BaseContext<FundingWalletEntity> context)
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
        public static void InitializePipeline(BaseContext<FundingVirtualAccountEntity> context)
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
