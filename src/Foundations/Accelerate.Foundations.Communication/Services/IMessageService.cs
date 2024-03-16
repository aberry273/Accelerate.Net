using Accelerate.Foundations.Communication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Communication.Services
{
    public interface IMessageServices
    {
        void SendEmail(string email, string subject, string message);
        Task SendEmailAsync(string email, string subject, string message);
        Task SendSmsAsync(string number, string message);
        void SendEmail(EmailMessage message);
        Task SendEmailAsync(EmailMessage message);
    }
}
