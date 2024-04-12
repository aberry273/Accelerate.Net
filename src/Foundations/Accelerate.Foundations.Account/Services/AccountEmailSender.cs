using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Communication.Models;
using Accelerate.Foundations.Communication.Services;
using System.Text.Encodings.Web;

namespace Accelerate.Foundations.Account.Services
{
    public class AccountEmailSender : Microsoft.AspNetCore.Identity.IEmailSender<AccountUser>
    {
        private string _emailTemplate { get; set; }
        private string _companyAddress = " Level 10/11 Britomart Place, Auckland CBD, Auckland 1010";
        private string _title = "Confirm your Parrot account";
        private string _heading = "Confirm your account";
        private string _emailCodeIntro = "Your 6 digit authentication code is.";
        private string _emailIntro = "A new account was created for this email.";
        private string _confirmAccountMessage = "Note: This will only be valid 30 days.";
        private string _emailpreHeader = "Confirm your password";
        private string _emailCta = "Click on the link below to set your password and confirm your account";
        private string _emailCtaLabel = "Confirm Account";

        IEmailSender _emailSender;
        public AccountEmailSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
            var path = Path.Combine("Templates", "Email/EmailTemplate.html");
            using (StreamReader reader = new StreamReader(path))
            {
                _emailTemplate = reader.ReadToEnd();
            }
        }
        
        public async Task SendConfirmationLinkAsync(AccountUser user, string email, string confirmationLink)
        {

            var name = user.AccountProfile != null ? $"{user.AccountProfile.Firstname} {user.AccountProfile.Lastname}" : user.UserName;
            var mailMessage = this.BuildEmailMessage(user.Email, name, _heading, _confirmAccountMessage, _emailCtaLabel, confirmationLink);
            await _emailSender.SendEmailAsync(mailMessage);
            /*
            var emailBody = $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>link</a>";
            var message = Foundations.Communication.Constants.Templates.Emails.CenteredTemplate.Replace("$$emailBody$$", emailBody);
            await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendConfirmationLinkAsync");
            */
        }

        public async Task SendPasswordResetCodeAsync(AccountUser user, string email, string resetCode)
        {
            var name = user.AccountProfile != null ? $"{user.AccountProfile.Firstname} {user.AccountProfile.Lastname}" : user.UserName;
            var mailMessage = this.BuildEmailCodeMessage(user.Email, "Your authentication code", "Your 6 digit code", _emailCodeIntro, resetCode);
            await _emailSender.SendEmailAsync(mailMessage);
            //await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendPasswordResetCodeAsync");
        }

        public async Task SendPasswordResetLinkAsync(AccountUser user, string email, string resetLink)
        {
            var emailBody = $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(resetLink)}'>link</a>";
            var message = Foundations.Communication.Constants.Templates.Emails.CenteredTemplate.Replace("$$emailBody$$", emailBody);
            await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendConfirmationLinkAsync");

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
        public EmailMessage BuildEmailMessage(string email, string name, string heading, string message, string ctaText, string ctaHref)
        {
            //var name = user.UserProfile != null ? $"{user.UserProfile.Firstname} {user.UserProfile.Lastname}" : user.UserName;

            var ctaStr = $"<a href=\"{ctaHref}\" target=\"_blank\" style=\"display: inline-block; color: #ffffff; background-color: #3498db; border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; text-decoration: none; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-transform: capitalize; border-color: #3498db;\">{ctaText}</a>";

            var str = _emailTemplate
                .Replace("$EmailMessage$", message)
                .Replace("$HtmlTitle$", _title)
                .Replace("$HeaderText$", _heading)
                .Replace("$EmailIntro$", _emailIntro)
                .Replace("$CallToAction$", ctaStr)
                .Replace("$CallToActionText$", _emailCta)
                .Replace("$PreheaderText$", _emailpreHeader)
                .Replace("CompanyAddress", _companyAddress);

            var model = new EmailMessage(new List<string>() { email }, _title, str);

            return model;
        }

    }
}
