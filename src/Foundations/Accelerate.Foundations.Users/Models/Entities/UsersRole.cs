using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Accelerate.Foundations.Users.Models.Entities
{
    public class UsersRole : IdentityRole<Guid>
    {
        public string Description { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedOn { get; set; }

        public virtual ICollection<UsersUserRole> UserRoles { get; set; }
        public virtual ICollection<UsersRoleClaim> RoleClaims { get; set; }
    }
}
