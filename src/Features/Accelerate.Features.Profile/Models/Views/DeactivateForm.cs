using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Profile.Models.Views
{
    public class DeactivateForm
    {

        //[EmailAddress]
        public Guid UserId { get; set; }

        //[EmailAddress]
        public string Username { get; set; }
    }
}
