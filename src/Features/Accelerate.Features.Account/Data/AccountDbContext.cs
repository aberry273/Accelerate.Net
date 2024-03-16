using Accelerate.Features.Account.Models;
using Accelerate.Features.Account.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Accelerator.Foundations.Users.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Accelerate.Features.Account.Data
{
    public class AccountDbContext : IdentityDbContext<AccountUser,
            AccountRole,
            Guid,
            AccountUserClaim,
            AccountUserRole,
            AccountUserLogin,
            AccountRoleClaim,
            AccountUserToken>
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options)
            : base(options)
        {
        }
        // Require empty constructor to enable migrations (when using external webapp project as a reference for DbContext)
        public AccountDbContext()
        {

        }
        public DbSet<AccountProfile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
           
            // Each User can have many UserClaims
            builder.Entity<AccountUser>()
                .HasMany(e => e.Claims)
                .WithOne()
                //.HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            builder.Entity<AccountUser>()
                .HasMany(e => e.Logins)
                .WithOne()
               // .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.Entity<AccountUser>()
                .HasMany(e => e.Tokens)
                .WithOne()
               // .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.Entity<AccountUser>()
                .HasMany(e => e.Roles)
                .WithOne(e => e.User)
              //  .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // Each user can have one Profile
            builder.Entity<AccountUser>()
                .HasOne(e => e.AccountProfile)
                .WithOne()
                //.HasForeignKey<AccountProfile>(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
          
            builder.Entity<AccountRole>()
                .HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
              //  .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<AccountRole>()
                .HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
               // .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        }
    }
}
