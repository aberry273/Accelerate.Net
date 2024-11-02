using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Integrations.Twilio.Services;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Accelerate.Foundations.Common.Models;
namespace Accelerate.Foundations.Integrations.Twilio
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
          
            services.Configure<TwilioConfiguration>(options =>
            {
                configuration.GetSection(Constants.Settings.SmsConfiguration).Bind(options);

                options.AccountSID = configuration[Constants.Twilio.AccountSID];
                options.AuthToken = configuration[Constants.Twilio.AuthToken];
            });
            services.AddTransient<ITwilioSmsSender, TwilioSmsSender>();
        }
    }
}
