using Accelerate.Foundations.Communication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Communication.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class MessageServices : IMessageServices
    {
        private IEmailSender _emailSender;
        public MessageServices(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public void SendEmail(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            _emailSender.SendEmail(email, subject, message);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            await _emailSender.SendEmailAsync(email, subject, message);
        }
        public void SendEmail(EmailMessage message)
        {
            // Plug in your email service here to send an email.
            _emailSender.SendEmail(message);
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            // Plug in your email service here to send an email.
            await _emailSender.SendEmailAsync(message);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
