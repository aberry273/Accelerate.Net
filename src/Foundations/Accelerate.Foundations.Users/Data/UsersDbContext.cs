using Accelerate.Foundations.Users.Models;
using Accelerate.Foundations.Users.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Users.Data
{
    public class UsersDbContext : IdentityDbContext<UsersUser,
            UsersRole,
            Guid,
            UsersUserClaim,
            UsersUserRole,
            UsersUserLogin,
            UsersRoleClaim,
            UsersUserToken>
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options)
            : base(options)
        {
        }
        // Require empty constructor to enable migrations (when using external webapp project as a reference for DbContext)
        public UsersDbContext()
        {

        }
        public DbSet<UsersUserLogin> UserLogins { get; set; }
        public DbSet<UsersProfile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
           
            // Each User can have many UserClaims
            builder.Entity<UsersUser>()
                .HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();
             
            // Each User can have many UserLogins
            builder.Entity<UsersUser>()
                .HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            builder.Entity<UsersUser>()
                .HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            builder.Entity<UsersUser>()
                .HasMany(e => e.Roles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            // Each user can have one Profile
            builder.Entity<UsersUser>()
                .HasOne(e => e.UsersProfile)
                .WithOne(e => e.User)
                .HasForeignKey<UsersProfile>(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
          
            builder.Entity<UsersRole>()
                .HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<UsersRole>()
                .HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        }
    }
}
