using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Accelerate.Foundations.Integrations.Twilio.Services
{
    public interface ITwilioSmsSender
    {
        Task<MessageResource> SendSmsAsync(string number, string message);
    }
}
