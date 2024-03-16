
using Accelerator.Foundation.Database;
using Accelerator.Foundation.Finance.Models;
using Accelerator.Foundation.Resources.Models;
using Accelerator.Foundations.Users.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Finance.Database
{
    public class FinanceDbContext : DbContext
    { 
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options,
                BaseContext<FinanceBudgetEntity> budgetContext,
                BaseContext<FinanceBudgetFeeEntity> budgetFeeContext,
                BaseContext<FinanceBudgetLineItemEntity> budgetLineItemsContext,
                BaseContext<FinanceQuoteEntity> quotesContext,
                BaseContext<FinanceQuoteLineItemEntity> quoteLineItemsContext,
                BaseContext<FinanceQuoteFeeEntity> quoteFeesContext,
                BaseContext<FinanceOrderEntity> orderContext,
                BaseContext<FinanceOrderLineItemEntity> orderLineItemsContext,
                BaseContext<FinanceOrderFeeEntity> orderFeesContext)
            : base(options)
        {
            Budgets = budgetContext;
            BudgetFees = budgetFeeContext;
            BudgetLineItems = budgetLineItemsContext;
            Quotes = quotesContext;
            QuoteLineItems = quoteLineItemsContext;
            QuoteFees = quoteFeesContext;
            Orders = orderContext;
            OrderLineItems = orderLineItemsContext;
            OrderFees = orderFeesContext;
        }
        public BaseContext<FinanceBudgetEntity> Budgets { get; set; }
        public BaseContext<FinanceBudgetLineItemEntity> BudgetLineItems { get; set; }
        public BaseContext<FinanceBudgetFeeEntity> BudgetFees { get; set; }

        public BaseContext<FinanceQuoteEntity> Quotes { get; set; }
        public BaseContext<FinanceQuoteLineItemEntity> QuoteLineItems { get; set; }
        public BaseContext<FinanceQuoteFeeEntity> QuoteFees { get; set; }

        public BaseContext<FinanceOrderEntity> Orders { get; set; }
        public BaseContext<FinanceOrderLineItemEntity> OrderLineItems { get; set; }
        public BaseContext<FinanceOrderFeeEntity> OrderFees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FinanceBudgetEntity>()
               .Property(p => p.Number)
               .ValueGeneratedOnAdd()
               .Metadata
               .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // Each Organisation can have many Contacts
            builder.Entity<FinanceBudgetEntity>()
                .HasMany(e => e.LineItems)
                .WithOne(e => e.Budget)
                .HasForeignKey(ut => ut.BudgetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FinanceBudgetEntity>()
                .HasMany(e => e.Fees)
                .WithOne(e => e.Budget)
                .HasForeignKey(ut => ut.BudgetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FinanceQuoteEntity>()
               .Property(p => p.Number)
               .ValueGeneratedOnAdd();

            builder.Entity<FinanceQuoteEntity>() 
                .HasMany(e => e.LineItems)
                .WithOne(e => e.Quote)
                .HasForeignKey(ut => ut.QuoteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FinanceQuoteEntity>()
                .Property(f => f.Number)
                .ValueGeneratedOnAdd();

            builder.Entity<FinanceQuoteEntity>()
                .HasMany(e => e.Fees)
                .WithOne(e => e.Quote)
                .HasForeignKey(ut => ut.QuoteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FinanceOrderEntity>()
                .HasMany(e => e.LineItems)
                .WithOne(e => e.Order)
                .HasForeignKey(ut => ut.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FinanceOrderEntity>()
                .HasMany(e => e.Fees)
                .WithOne(e => e.Order)
                .HasForeignKey(ut => ut.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Ignore<ResourceItemEntity>();
            builder.Ignore<ResourceFeeEntity>();
        }
    }
}
