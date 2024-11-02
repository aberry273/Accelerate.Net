using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Accounts.Database
{
    public class AccountsDbContext : DbContext
    {
        public BaseContext<AccountsBusinessEntity> Businesses { get; set; }
        public BaseContext<AccountsIndividualEntity> Individuals { get; set; }
        public BaseContext<AccountsContactEntity> Contacts { get; set; }
        public BaseContext<AccountsAddressEntity> Addresses { get; set; }
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options,
                BaseContext<AccountsBusinessEntity> accountContext,
                BaseContext<AccountsIndividualEntity> individualContext,
                BaseContext<AccountsContactEntity> contactContext,
                BaseContext<AccountsAddressEntity> addressContext
            )
            : base(options)
        {
            Businesses = accountContext;
            Individuals = individualContext;
            Contacts = contactContext;
            Addresses = addressContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AccountsBusinessEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsIndividualEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsContactEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsAddressEntity>().HasKey(c => c.Id);

            //builder.Entity<AccountsAccountBankAccountEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsAccountContactEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsAccountAddressEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsAccountChildEntity>().HasKey(c => c.Id);

            // Accounts
            builder.Entity<AccountsBusinessEntity>()
               .HasMany(e => e.Contacts)
                 .WithOne(x => x.AccountsAccount)
                 .HasForeignKey(x => x.AccountsAccountId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AccountsBusinessEntity>()
               .HasMany(e => e.ChildAccounts)
                 .WithOne(x => x.AccountsAccount)
                 .HasForeignKey(x => x.AccountsAccountId)
                 .OnDelete(DeleteBehavior.Cascade);
            /*
            builder.Entity<AccountsAccountEntity>()
               .HasOne(e => e.BankAccount)
                 .WithOne(x => x.AccountsAccount)
                 .OnDelete(DeleteBehavior.Cascade);
            */
        }
    }
}
