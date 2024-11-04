using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace Accelerate.Foundations.Common
{
    public static class Startup
    {
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SiteConfiguration>(configuration.GetSection(Constants.Settings.SiteConfiguration).Bind);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            services.AddTransient<IMetaContentService, MetaContentService>();
            services.AddTransient<ISharedContentService, SharedContentService>();
            services.AddTransient<IRssReaderService, RssReaderService>();
            //services.AddTransient<IResilientHttpClient, ResilientHttpClient>();

            //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0

            services.AddHttpClient<IResilientHttpClient, ResilientHttpClient>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(2))  //Set lifetime to two minutes
                    .AddPolicyHandler(GetRetryPolicy());
         
        }
    }
}
