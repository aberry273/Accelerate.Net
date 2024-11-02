using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Accounts.Models.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
   
    [Table("AccountsAddress")]
    public class AccountsAddressEntity : AccountsBaseEntity
    {
        // Address
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public int Postcode { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? Country { get; set; }
    }
}
