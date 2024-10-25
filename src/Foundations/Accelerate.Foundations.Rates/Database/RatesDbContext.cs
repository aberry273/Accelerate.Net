using Accelerate.Foundations.Rates.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Rates.Database
{
    public class RatesDbContext : DbContext
    {
        public BaseContext<RatesCustomerEntity> Customers { get; set; }
        public BaseContext<RatesConversionQuoteEntity> Quote { get; set; }
        public BaseContext<RatesConversionOrderEntity> Orders { get; set; }
        public RatesDbContext(DbContextOptions<RatesDbContext> options,
                BaseContext<RatesCustomerEntity> customerContext,
                BaseContext<RatesConversionQuoteEntity> quoteContext,
                BaseContext<RatesConversionOrderEntity> orderContext
            )
            : base(options)
        {
            Customers = customerContext;
            Quote = quoteContext;
            Orders = orderContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<RatesCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<RatesConversionQuoteEntity>().HasKey(c => c.Id);
            builder.Entity<RatesConversionOrderEntity>().HasKey(c => c.Id);
        }
    }
}
