
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Integrations.Elastic.Services;

namespace Accelerate.Foundations.Integrations.Elastic
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // CONFIGS
            // SERVICES
            services.AddTransient<IKendentpayService, KendentpayService>();
        }
    }
}
