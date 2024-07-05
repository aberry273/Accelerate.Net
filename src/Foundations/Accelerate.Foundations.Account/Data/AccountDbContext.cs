using Accelerate.Foundations.Account.Models;
using Accelerate.Foundations.Account.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Account.Data
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
        public DbSet<AccountUserLogin> UserLogins { get; set; }
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
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
             
            // Each User can have many UserLogins
            builder.Entity<AccountUser>()
                .HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.Entity<AccountUser>()
                .HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.Entity<AccountUser>()
                .HasMany(e => e.Roles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // Each user can have one Profile
            builder.Entity<AccountUser>()
                .HasOne(e => e.AccountProfile)
                .WithOne(e => e.User)
                .HasForeignKey<AccountProfile>(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
          
            builder.Entity<AccountRole>()
                .HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<AccountRole>()
                .HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        }
    }
}
