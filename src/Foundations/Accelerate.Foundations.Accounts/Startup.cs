
using Accelerate.Foundations.Accounts.Database;
using Accelerate.Foundations.Accounts.Models;
using Accelerate.Foundations.Accounts.Models.Entities;
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

namespace Accelerate.Foundations.Accounts
{
    public static class Startup
    {
        public static void ConfigureCommands(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            ///Mass transit
            //services.AddMediator(x => x.AddConsumersFromNamespaceContaining<AccountsAddressEntity>());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, bool isProduction)
        {
            //get secret
            // CONFIGS
            services.Configure<AccountsConfiguration>(options =>
            {
                configuration.GetSection(Constants.Config.ConfigName).Bind(options);
            });
            var connString = isProduction ? configuration[Constants.Config.DatabaseKey] : configuration.GetConnectionString(Constants.Config.LocalDatabaseKey);
            // Core contexts
            services.AddDbContext<BaseContext<AccountsBusinessEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsIndividualEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsAddressEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsContactEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            // Function contexts
            //services.AddDbContext<BaseContext<AccountsAccountBankAccountEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsAccountChildEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<AccountsAccountContactEntity>>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);

            //Services
            services.AddTransient<IEntityService<AccountsBusinessEntity>, EntityService<AccountsBusinessEntity>>();
            services.AddTransient<IEntityService<AccountsIndividualEntity>, EntityService<AccountsIndividualEntity>>();
            services.AddTransient<IEntityService<AccountsAddressEntity>, EntityService<AccountsAddressEntity>>();
            services.AddTransient<IEntityService<AccountsContactEntity>, EntityService<AccountsContactEntity>>();

            //CQRS
            ConfigureCommands(services, configuration);

            // Logic
            //services.AddTransient<IContentPostService, ContentPostService>();

            //Parent context for mappings
            services.AddDbContext<AccountsDbContext>(options => options.UseSqlServer(connString), ServiceLifetime.Transient);


        }
        public static void InitializePipeline(BaseContext<AccountsBusinessEntity> context)
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
        /*
        public static void InitializePipeline(BaseContext<AccountsBankAccountEntity> context)
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
        */
    }
}
