using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Authentication.Models.Views
{
    public class DeactivateForm
    {

        //[EmailAddress]
        public Guid UserId { get; set; }

        //[EmailAddress]
        public string Username { get; set; }
    }
}
