using Accelerate.Foundations.Communication.Models;
using Azure.Communication.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Cms;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Twilio.TwiML.Messaging;
using systemMail = System.Net.Mail;
namespace Accelerate.Foundations.Communication.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(
          ILogger<EmailSender> logger, IOptions<EmailConfiguration> emailConfig)
        {
            _logger = logger;
            _emailConfig = emailConfig.Value;
        }
        public EmailSender(EmailConfiguration emailConfig)
        {
            _logger = LoggerFactory.Create(builder =>
                {
                    builder
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug);
                })
                .CreateLogger<EmailSender>();

            _emailConfig = emailConfig;
        }

        public void SendEmail(Models.EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task SendEmailAsync(Models.EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        } 
        public void SendEmail(string email, string subject, string message)
        {
            var modal = new Models.EmailMessage(new List<string>() { email }, subject, message);
            this.SendEmail(modal);
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var modal = new Models.EmailMessage(new List<string>() { email }, subject, message);
            await this.SendEmailAsync(modal);
        }
        private MimeMessage CreateEmailMessage(Models.EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(message.Subject, _emailConfig.From));
            emailMessage.To.AddRange(message.To); 
            emailMessage.Subject = message.Subject;
            var multipart = new Multipart("mixed");
            multipart.Add(new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content });
            if(message.Attachments != null)
            {
                foreach (var attachment in message.Attachments)
                {
                    multipart.Add(attachment);
                }
            }
            emailMessage.Body = multipart;
            return emailMessage;
        }

        public MimePart CreateMimeAttachment(string mediaType, string fileType, byte[] file, string fileName)
        {
            MemoryStream stream = new MemoryStream();
            stream.Write(file, 0, file.Length);

            // create an image attachment for the file located at path
            var attachment = new MimePart(mediaType, fileType)
            {
                Content = new MimeContent(stream, ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };
            return attachment;
        }
        public void Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.LocalDomain = _emailConfig.FromDomain;
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                var ssl = _emailConfig.Port == 465;

                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, ssl);

                client.Authenticate(_emailConfig.Username, _emailConfig.Password);

                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
        public async Task SendAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();

            try
            {
                client.LocalDomain = _emailConfig.FromDomain;
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls).ConfigureAwait(false);

                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password).ConfigureAwait(false);

                await client.SendAsync(mailMessage).ConfigureAwait(false);

                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
