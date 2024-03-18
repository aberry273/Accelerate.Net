using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accelerate.Foundations.Account.Models.Entities
{
    public class AccountRole : IdentityRole<Guid>
    {
        public string Description { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedOn { get; set; }

        public virtual ICollection<AccountUserRole> UserRoles { get; set; }
        public virtual ICollection<AccountRoleClaim> RoleClaims { get; set; }
    }
}
