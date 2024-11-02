using Accelerate.Foundations.Funding.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Funding.Database
{
    public class FundingDbContext : DbContext
    {
        public BaseContext<FundingWalletEntity> Wallets { get; set; }
        public BaseContext<FundingVirtualAccountEntity> VirtualAccounts { get; set; }
        public BaseContext<FundingBankAccountEntity> BankAccounts { get; set; }
        public BaseContext<FundingSourceEntity> Sources { get; set; }
        public BaseContext<FundingCustomerEntity> Customers { get; set; }
        public FundingDbContext(DbContextOptions<FundingDbContext> options,
                BaseContext<FundingWalletEntity> walletsContext,
                BaseContext<FundingVirtualAccountEntity> virtualAccountsContext,
                BaseContext<FundingBankAccountEntity> bankAccountContext,
                BaseContext<FundingSourceEntity> sourceContext,
                BaseContext<FundingCustomerEntity> customerContext
            )
            : base(options)
        {
            BankAccounts = bankAccountContext;
            VirtualAccounts = virtualAccountsContext;
            Wallets = walletsContext;
            Sources = sourceContext;
            Customers = customerContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<FundingWalletEntity>().HasKey(c => c.Id);
            builder.Entity<FundingBankAccountEntity>().HasKey(c => c.Id);
            builder.Entity<FundingBankAccountEntity>().HasKey(c => c.Id);
            builder.Entity<FundingSourceEntity>().HasKey(c => c.Id);
            builder.Entity<FundingCustomerEntity>().HasKey(c => c.Id);

        }
    }
}
