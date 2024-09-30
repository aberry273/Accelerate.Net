using Accelerate.Foundations.Common.Models;
using Accelerate.Foundations.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace Accelerate.Foundations.Common
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SiteConfiguration>(configuration.GetSection(Constants.Settings.SiteConfiguration).Bind);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelperFactory, UrlHelperFactory>();

            services.AddTransient<IMetaContentService, MetaContentService>();
            services.AddTransient<IRssReaderService, RssReaderService>();
        }
    }
}
