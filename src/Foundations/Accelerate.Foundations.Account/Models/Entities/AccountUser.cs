using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Accelerate.Foundations.Database.Models;

namespace Accelerate.Foundations.Account.Models.Entities
{
    public enum AccountUserStatus
    {
        Active, Deactivated, Deleted
    }
    public class AccountUser : IdentityUser<Guid>, IBaseEntity
    {
        [Required, MinLength(3), MaxLength(20)]
        public override string UserName { get; set; }

        public string Domain { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedOn { get; set; }

        public Guid AccountProfileId { get; set; }
        public AccountUserStatus Status { get; set; }
        public virtual AccountProfile AccountProfile { get; set; }
        public virtual ICollection<AccountUserRole> Roles { get; } = new List<AccountUserRole>();

        public virtual ICollection<AccountUserClaim> Claims { get; } = new List<AccountUserClaim>();

        public virtual ICollection<AccountUserLogin> Logins { get; } = new List<AccountUserLogin>();

        public virtual ICollection<AccountUserToken> Tokens { get; } = new List<AccountUserToken>();
    }
}
