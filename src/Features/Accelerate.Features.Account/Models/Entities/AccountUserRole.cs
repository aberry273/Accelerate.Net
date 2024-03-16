namespace Accelerate.Features.Account.Models.Entities
{
    public class AccountUserRole : Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>
    {

        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual AccountUser User { get; set; }
        public virtual AccountRole Role { get; set; }
    }
}
