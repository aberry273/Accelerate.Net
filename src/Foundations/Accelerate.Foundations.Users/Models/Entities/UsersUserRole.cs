namespace Accelerate.Foundations.Users.Models.Entities
{
    public class UsersUserRole : Microsoft.AspNetCore.Identity.IdentityUserRole<Guid>
    {

        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual UsersUser User { get; set; }
        public virtual UsersRole Role { get; set; }
    }
}
