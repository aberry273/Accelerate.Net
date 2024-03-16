using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Integrations.Twilio.Services;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace Accelerate.Foundations.Communication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add configs
               /*
            var emailConfig = configuration
                .GetSection(Accelerate.Foundations.Communication.Constants.Settings.EmailConfiguration)
                .Get<EmailConfiguration>();         
            var smsConfig = configuration
                .GetSection(Accelerate.Foundations.Communication.Constants.Settings.SmsConfiguration)
                .Get<TwilioConfiguration>();

            services.AddSingleton(emailConfig);
            services.Configure< TwilioConfiguration>((smsConfig));
            */
            services.Configure<IOptions<EmailConfiguration>>(configuration.GetSection(Foundations.Communication.Constants.Settings.EmailConfiguration));
            services.Configure<IOptions<TwilioConfiguration>>(configuration.GetSection(Foundations.Communication.Constants.Settings.SmsConfiguration));

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMessageServices, MessageServices>();
            services.AddTransient<ISmsSender<MessageResource>, SmsSender>();
        }
    }
}
