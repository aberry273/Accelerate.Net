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
using Accelerate.Foundations.Portal.Services;

namespace Accelerate.Foundations.Portal
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<SiteConfiguration>(configuration.GetSection(Constants.Settings.SiteConfiguration).Bind);
            services.AddTransient<IPortalContentService, PortalContentService>();
            services.AddTransient<IPortalSessionService, PortalSessionService>();


        }
    }
}
