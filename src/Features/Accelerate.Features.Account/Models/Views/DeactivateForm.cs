using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Account.Models.Views
{
    public class DeactivateForm
    {

        //[EmailAddress]
        public Guid UserId { get; set; }

        //[EmailAddress]
        public string Username { get; set; }
    }
}
