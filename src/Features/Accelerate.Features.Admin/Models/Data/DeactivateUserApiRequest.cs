using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Admin.Models.Data
{
    public class DeactivateUserApiRequest
    {

        //[EmailAddress]
        public Guid UserId { get; set; }

        //[EmailAddress]
        public string Username { get; set; }
    }
}
