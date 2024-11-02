using Accelerate.Foundations.Transactions.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Transactions.Database
{
    public class TransactionsDbContext : DbContext
    {
        public BaseContext<TransactionsCustomerEntity> Customers { get; set; }
        public BaseContext<TransactionsTransactionEntity> Transactions { get; set; }
        //public BaseContext<TransactionsWalletEntity> Wallets { get; set; }
        public BaseContext<TransactionsAddressEntity> Addresses { get; set; }
        public TransactionsDbContext(DbContextOptions<TransactionsDbContext> options,
                BaseContext<TransactionsCustomerEntity> customerContext,
                BaseContext<TransactionsTransactionEntity> transactionContext,
                //BaseContext<TransactionsWalletEntity> walletContext,
                BaseContext<TransactionsAddressEntity> addressContext
            )
            : base(options)
        {
            Customers = customerContext;
            Transactions = transactionContext;
            //Wallets = walletContext;
            Addresses = addressContext; 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TransactionsCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<TransactionsTransactionEntity>().HasKey(c => c.Id);
            //builder.Entity<TransactionsWalletEntity>().HasKey(c => c.Id);
            builder.Entity<TransactionsAddressEntity>().HasKey(c => c.Id);


        }
    }
}
