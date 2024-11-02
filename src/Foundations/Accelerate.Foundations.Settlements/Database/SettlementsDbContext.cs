using Accelerate.Foundations.Settlements.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Settlements.Database
{
    public class SettlementsDbContext : DbContext
    {
        public BaseContext<SettlementsLedgerEntity> Ledgers { get; set; }
        public BaseContext<SettlementsLedgerStatementEntity> LedgerStatements { get; set; }
        public BaseContext<SettlementsLedgerTransactionEntity> LedgerTransactions { get; set; }
        public SettlementsDbContext(DbContextOptions<SettlementsDbContext> options,
                BaseContext<SettlementsLedgerEntity> walletsContext,
                BaseContext<SettlementsLedgerStatementEntity> ledgerStatementContext,
                BaseContext<SettlementsLedgerTransactionEntity> bankAccountContext
            )
            : base(options)
        {
            Ledgers = walletsContext;
            LedgerStatements = ledgerStatementContext;
            LedgerTransactions = bankAccountContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<SettlementsLedgerEntity>().HasKey(c => c.Id);
            builder.Entity<SettlementsLedgerStatementEntity>().HasKey(c => c.Id);
            builder.Entity<SettlementsLedgerTransactionEntity>().HasKey(c => c.Id);

        }
    }
}
