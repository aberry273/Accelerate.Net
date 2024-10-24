using Accelerate.Foundations.Accounts.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Accounts.Database
{
    public class AccountsDbContext : DbContext
    {
        public BaseContext<AccountsCustomerEntity> Customers { get; set; }
        public BaseContext<AccountsBankAccountEntity> BankAccounts { get; set; }
        public AccountsDbContext(DbContextOptions<AccountsDbContext> options,
                BaseContext<AccountsCustomerEntity> customerContext,
                BaseContext<AccountsBankAccountEntity> bankAccountContext
            )
            : base(options)
        {
            Customers = customerContext;
            BankAccounts = bankAccountContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AccountsCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<AccountsBankAccountEntity>().HasKey(c => c.Id);

        }
    }
}
