using Accelerate.Foundations.Integrations.Twilio.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Accelerate.Foundations.Integrations.Twilio.Services
{
    public class SmsSender : ISmsSender<MessageResource>
        {
            private readonly ILogger<SmsSender> _logger;
            private readonly TwilioConfiguration _twilioConfig;
            private readonly string _from;
            public SmsSender(
              ILogger<SmsSender> logger, IOptions<TwilioConfiguration> twilioConfig)
            {
                _logger = logger;
                _twilioConfig = twilioConfig.Value;

                TwilioClient.Init(_twilioConfig.AccountSID, _twilioConfig.AuthToken);
            }

            public async Task<MessageResource> SendSmsAsync(string to, string body)
            {
                return await this.SendSmsAsync(to, _from, body);
            }
            public async Task<MessageResource> SendSmsAsync(string to, string from, string body)
            {
                var message = new SmsMessage(to, from, body);
                return await MessageResource.CreateAsync(
                    body: message.Content,
                    from: message.From,
                    to: message.To
                );
            }
        }
}
