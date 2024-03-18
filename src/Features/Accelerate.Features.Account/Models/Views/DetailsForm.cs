using System.ComponentModel.DataAnnotations;

namespace Accelerate.Features.Account.Models.Views
{
    public class DetailsForm
    {
        //[EmailAddress]
        public Guid Id { get; set; }
        public string? Response { get; set; }
        public bool IsEmailConfirmed { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        public string StatusMessage { get; set; }

        // Personal
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        // Contact
        [EmailAddress]
        public string SecondaryEmail { get; set; }
        [Phone]
        public string SecondaryNumber { get; set; }
        // Address
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        // Other
        public string Position { get; set; }
    }
}
