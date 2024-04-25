using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Account.Models.Data
{
    public class ResetPasswordForm
    {
        [Required]
        //[EmailAddress]
        public string UserId { get; set; }

        [Required]
        public string Code { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }

        public string? Response { get; set; }
    }
}
