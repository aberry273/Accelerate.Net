using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Account.Models.Data
{
    public class ForgotPasswordForm
    {
        [Required]
        //[EmailAddress]
        public string Username { get; set; }
        public string? Response { get; set; }
    }
}
