using Accelerate.Foundations.Communication.Models;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Communication.Services
{
    public interface IEmailSender
    {
        public void SendEmail(EmailMessage message);
        public Task SendEmailAsync(EmailMessage message);
        public void SendEmail(string email, string subject, string message);
        public Task SendEmailAsync(string email, string subject, string message);
        MimePart CreateMimeAttachment(string mediaType, string fileType, byte[] file, string fileName);
    }
}
