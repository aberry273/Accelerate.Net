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
        public BaseContext<TransfersPayinEntity> Payins { get; set; }
        public BaseContext<TransfersPayoutEntity> Payouts { get; set; }
        public TransfersDbContext(DbContextOptions<TransfersDbContext> options,
                BaseContext<TransfersCustomerEntity> customerContext,
                BaseContext<TransfersPayinEntity> payinsContext,
                BaseContext<TransfersPayoutEntity> payoutsContext
            )
            : base(options)
        {
            Customers = customerContext;
            Payins = payinsContext;
            Payouts = payoutsContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<TransfersCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<TransfersPayinEntity>().HasKey(c => c.Id);
            builder.Entity<TransfersPayoutEntity>().HasKey(c => c.Id);
        }
    }
}
