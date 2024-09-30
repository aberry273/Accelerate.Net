using Accelerate.Foundations.Integrations.Quartz.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Integrations.Quartz
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            //services.AddSingleton(emailConfig);

            services.AddTransient<IQuartzService, QuartzService>();
        }
    }
}
