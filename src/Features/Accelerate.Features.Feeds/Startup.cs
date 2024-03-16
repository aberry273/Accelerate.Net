using Microsoft.AspNetCore.Authentication;

namespace Accelerate.Features.Feeds
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            /*
            services.AddDbContext<BaseContext<FinanceQuoteEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceQuoteLineItemEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceQuoteFeeEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);

            services.AddDbContext<BaseContext<FinanceOrderEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceOrderLineItemEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceOrderFeeEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);

            services.AddDbContext<BaseContext<FinanceBudgetEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceBudgetLineItemEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);
            services.AddDbContext<BaseContext<FinanceBudgetFeeEntity>>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);

            services.AddDbContext<FinanceDbContext>(options => options.UseSqlServer(configuration.GetConnectionString(Foundations.Common.Constants.Site.Settings.ConnectionStringName)), ServiceLifetime.Transient);

            services.AddTransient<IEntityService<FinanceQuoteEntity>, EntityService<FinanceQuoteEntity>>();
            services.AddTransient<IEntityService<FinanceQuoteLineItemEntity>, EntityService<FinanceQuoteLineItemEntity>>();
            services.AddTransient<IEntityService<FinanceQuoteFeeEntity>, EntityService<FinanceQuoteFeeEntity>>();

            services.AddTransient<IEntityService<FinanceOrderEntity>, EntityService<FinanceOrderEntity>>();
            services.AddTransient<IEntityService<FinanceOrderLineItemEntity>, EntityService<FinanceOrderLineItemEntity>>();
            services.AddTransient<IEntityService<FinanceOrderFeeEntity>, EntityService<FinanceOrderFeeEntity>>();

            services.AddTransient<IEntityService<FinanceBudgetEntity>, EntityService<FinanceBudgetEntity>>();
            services.AddTransient<IEntityService<FinanceBudgetLineItemEntity>, EntityService<FinanceBudgetLineItemEntity>>();
            services.AddTransient<IEntityService<FinanceBudgetFeeEntity>, EntityService<FinanceBudgetFeeEntity>>();

            services.AddTransient<IFinanceQuoteService, FinanceQuoteService>();
            services.AddTransient<IFinanceQuoteFeeService, FinanceQuoteFeeService>();
            services.AddTransient<IFinanceQuoteLineItemService, FinanceQuoteLineItemService>();

            services.AddTransient<IFinanceOrderService, FinanceOrderService>();
            services.AddTransient<IFinanceOrderFeeService, FinanceOrderFeeService>();
            services.AddTransient<IFinanceOrderLineItemService, FinanceOrderLineItemService>();

            services.AddTransient<IFinanceBudgetService, FinanceBudgetService>();
            services.AddTransient<IFinanceBudgetLineItemService, FinanceBudgetLineItemService>();
            services.AddTransient<IFinanceBudgetFeeService, FinanceBudgetFeeService>();
            */
        }
        /*
        public static void InitializeDb(FinanceDbContext context)
        {
            try
            {
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        */
    }
}
