using Accelerate.Features.Account.Models.Entities;
using Accelerate.Foundations.Communication.Services;

namespace Accelerate.Features.Account.Services
{
    public class AccountEmailSender : Microsoft.AspNetCore.Identity.IEmailSender<AccountUser>
    {
        /*
        IEmailSender _emailSender;
        AccountEmailSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        */
        public async Task SendConfirmationLinkAsync(AccountUser user, string email, string confirmationLink)
        {
            //await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendConfirmationLinkAsync");
        }

        public async Task SendPasswordResetCodeAsync(AccountUser user, string email, string resetCode)
        {
            //await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendPasswordResetCodeAsync");
        }

        public async Task SendPasswordResetLinkAsync(AccountUser user, string email, string resetLink)
        {
            //await _emailSender.SendEmailAsync(email, "Account Confirmation", "TODO: SendPasswordResetLinkAsync");
        }
    }
}
