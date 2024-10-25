using Accelerate.Foundations.Orders.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Orders.Database
{
    public class OrdersDbContext : DbContext
    {
        public BaseContext<OrdersCustomerEntity> Customers { get; set; }
        public BaseContext<OrdersPurchaseOrderEntity> Orders { get; set; }
        public BaseContext<OrdersPurchaseQuoteEntity> Quotes { get; set; }
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options,
                BaseContext<OrdersCustomerEntity> customerContext,
                BaseContext<OrdersPurchaseOrderEntity> ordersContext,
                BaseContext<OrdersPurchaseQuoteEntity> quotesContext
            )
            : base(options)
        {
            Customers = customerContext;
            Orders = ordersContext;
            Quotes = quotesContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OrdersCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<OrdersPurchaseOrderEntity>().HasKey(c => c.Id);
            builder.Entity<OrdersPurchaseQuoteEntity>().HasKey(c => c.Id);
        }
    }
}
