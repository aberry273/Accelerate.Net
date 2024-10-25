using Microsoft.Extensions.DependencyInjection;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using Accelerate.Foundations.Integrations.Twilio.Models;
using Accelerate.Foundations.Integrations.Twilio.Services;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Accelerate.Foundations.Common.Models;
namespace Accelerate.Foundations.Communication
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add configs
            services.Configure<EmailConfiguration>(options =>
            {
                configuration.GetSection(Constants.Settings.EmailConfiguration).Bind(options);

                //options.ConnectionString = configuration[Constants.AzureCommunicationServices.ConnectionString];
                options.Username = configuration[Constants.AzureCommunicationServices.EmailCommunicationServiceName];
                options.Password = configuration[Constants.AzureCommunicationServices.EmailCommunicationServiceSecret];
            });
            services.Configure<TwilioConfiguration>(options =>
            {
                configuration.GetSection(Constants.Settings.SmsConfiguration).Bind(options);

                options.AccountSID = configuration[Constants.Twilio.AccountSID];
                options.AuthToken = configuration[Constants.Twilio.AuthToken];
            });
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IMessageServices, MessageServices>();
            services.AddTransient<ISmsSender<MessageResource>, SmsSender>();
        }
    }
}
