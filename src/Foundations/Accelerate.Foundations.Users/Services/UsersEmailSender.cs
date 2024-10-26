using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using System.Text.Encodings.Web;

namespace Accelerate.Foundations.Users.Services
{
    public class UsersEmailSender : Microsoft.AspNetCore.Identity.IEmailSender<UsersUser>
    {
        private string _emailTemplate { get; set; }
        private string _companyAddress = " Level 10/11 Britomart Place, Auckland CBD, Auckland 1010";
        private string _title = "Confirm your Parrot Users";
        private string _heading = "Confirm your Users";
        private string _emailCodeIntro = "Your 6 digit authentication code is.";
        private string _emailIntro = "A new Users was created for this email.";
        private string _confirmUsersMessage = "Note: This will only be valid 30 days.";
        private string _emailpreHeader = "Confirm your password";
        private string _emailCta = "Click on the link below to set your password and confirm your Users";
        private string _emailCtaLabel = "Confirm Users";

        IEmailSender _emailSender;
        public UsersEmailSender(IEmailSender emailSender)
        {
            try
            {
                _emailSender = emailSender;
                var path = Path.Combine("Templates", "Email/EmailTemplate.html");
                using (StreamReader reader = new StreamReader(path))
                {
                    _emailTemplate = reader.ReadToEnd();
                }
            }
            catch(Exception ex)
            {
                Foundations.Common.Services.StaticLoggingService.LogError(ex);
            }
        }
        
        public async Task SendConfirmationLinkAsync(UsersUser user, string ctaHref, string ctaLabel)
        {

            var name = user.UsersProfile != null ? $"{user.UsersProfile.Firstname} {user.UsersProfile.Lastname}" : user.UserName;
            var intro = $"Hi, {name}. Click on the link below to confirm your new Users";

            var ctaStr = $"<a href=\"{ctaHref}\" target=\"_blank\" style=\"display: inline-block; color: #ffffff; background-color: #3498db; border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-transform: capitalize; border-color: #3498db;\">{ctaLabel}</a>";

            //var emailBody = $"Please confirm your Users by clicking this link: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>link</a>";
            var mailMessage = this.BuildEmailMessage(user.Email, intro, _heading, _confirmUsersMessage, ctaStr);
            await _emailSender.SendEmailAsync(mailMessage);
            /*
            var emailBody = $"Please confirm your Users by clicking this link: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>link</a>";
            var message = Foundations.Communication.Constants.Templates.Emails.CenteredTemplate.Replace("$$emailBody$$", emailBody);
            await _emailSender.SendEmailAsync(email, "Users Confirmation", "TODO: SendConfirmationLinkAsync");
            */
        }

        public async Task SendPasswordResetCodeAsync(UsersUser user, string email, string resetCode)
        {
            var name = user.UsersProfile != null ? $"{user.UsersProfile.Firstname} {user.UsersProfile.Lastname}" : user.UserName;
            var mailMessage = this.BuildEmailCodeMessage(user.Email, "Your authentication code", "Your 6 digit code", _emailCodeIntro, resetCode);
            await _emailSender.SendEmailAsync(mailMessage);
            //await _emailSender.SendEmailAsync(email, "Users Confirmation", "TODO: SendPasswordResetCodeAsync");
        }

        public async Task SendPasswordResetLinkAsync(UsersUser user, string email, string resetLink)
        {
            var emailBody = $"Please confirm your Users by clicking this link: <a href='{HtmlEncoder.Default.Encode(resetLink)}'>link</a>";
            var message = Foundations.Communication.Constants.Templates.Emails.CenteredTemplate.Replace("$$emailBody$$", emailBody);
            await _emailSender.SendEmailAsync(email, "Users Confirmation", "TODO: SendConfirmationLinkAsync");

        }
        public EmailMessage BuildEmailCodeMessage(string email, string title, string heading, string message, string code)
        {
            //var name = user.UserProfile != null ? $"{user.UserProfile.Firstname} {user.UserProfile.Lastname}" : user.UserName;

            var str = _emailTemplate
                .Replace("$EmailMessage$", message)
                .Replace("$HtmlTitle$", title)
                .Replace("$HeaderText$", heading)
                .Replace("$CallToAction$", code)
                .Replace("$EmailIntro$", _emailIntro)
                .Replace("$PreheaderText$", _emailpreHeader)
                .Replace("CompanyAddress", _companyAddress);

            var model = new EmailMessage(new List<string>() { email }, _title, str);

            return model;
        }
        public EmailMessage BuildEmailMessage(string email, string intro, string heading, string message, string ctaStr)
        {
            //var name = user.UserProfile != null ? $"{user.UserProfile.Firstname} {user.UserProfile.Lastname}" : user.UserName;

            var str = _emailTemplate
                .Replace("$EmailMessage$", message)
                .Replace("$HtmlTitle$", _title)
                .Replace("$HeaderText$", heading)
                .Replace("$EmailIntro$", intro)
                .Replace("$CallToAction$", ctaStr)
                .Replace("$PreheaderText$", _emailpreHeader)
                .Replace("CompanyAddress", _companyAddress);

            var model = new EmailMessage(new List<string>() { email }, _title, str);

            return model;
        }

    }
}
