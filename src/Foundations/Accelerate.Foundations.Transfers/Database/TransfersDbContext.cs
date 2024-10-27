using Accelerate.Foundations.Transfers.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Transfers.Database
{
    public class TransfersDbContext : DbContext
    {
        public BaseContext<TransfersCustomerEntity> Customers { get; set; }
        public BaseContext<TransfersTransactions> Transactions { get; set; }
        public TransfersDbContext(DbContextOptions<TransfersDbContext> options,
                BaseContext<TransfersCustomerEntity> customerContext,
                BaseContext<TransfersTransactions> transactionsContext 
            )
            : base(options)
        {
            Customers = customerContext;
            Transactions = transactionsContext; 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TransfersCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<TransfersTransactions>().HasKey(c => c.Id);
        }
    }
}
