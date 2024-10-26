using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Foundations.Users.Models.Entities
{
    public enum UsersUserStatus
    {
        Active, Deactivated, Deleted
    }
    public class UsersUser : IdentityUser<Guid>, IBaseEntity
    {
        [Required, MinLength(3), MaxLength(20)]
        public override string UserName { get; set; }

        public string Domain { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        public Guid? UsersProfileId { get; set; }
        public UsersUserStatus Status { get; set; }
        public virtual UsersProfile? UsersProfile { get; set; }
        public virtual ICollection<UsersUserRole> Roles { get; } = new List<UsersUserRole>();

        public virtual ICollection<UsersUserClaim> Claims { get; } = new List<UsersUserClaim>();

        public virtual ICollection<UsersUserLogin> Logins { get; } = new List<UsersUserLogin>();

        public virtual ICollection<UsersUserToken> Tokens { get; } = new List<UsersUserToken>();
    }
}
