
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Integrations.Kindentpay.Services;

namespace Accelerate.Foundations.Integrations.Kindentpay
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
