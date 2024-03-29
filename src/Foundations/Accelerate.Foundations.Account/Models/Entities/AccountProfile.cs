using Accelerate.Foundations.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Account.Models.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class

    [Table("AccountProfiles")]
    public class AccountProfile : BaseEntity
    {
        #region Required 
        #endregion
        public string? Title { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? SecondaryNumber { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Address { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? Postcode { get; set; }
        public string? Country { get; set; }
        public string? Position { get; set; }
        public AccountUser User { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        [NotMapped]
        public IEnumerable<string> TagItems
        {
            get
            {
                return Tags?.Split(',')?.Select(x => x.Trim()).ToList();
            }
            set
            {
                if (value != null) Tags = string.Join(',', value?.Select(x => x.Trim()));
            }
        }
    }
}
