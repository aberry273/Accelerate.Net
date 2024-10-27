using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Authentication.Models.Data
{
    public class ForgotPasswordForm
    {
        [Required]
        //[EmailAddress]
        public string Username { get; set; }
        public string? Response { get; set; }
    }
}
