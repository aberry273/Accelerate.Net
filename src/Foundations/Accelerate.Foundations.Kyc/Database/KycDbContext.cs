using Accelerate.Foundations.Kyc.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Accelerate.Foundations.Kyc.Database
{
    public class KycDbContext : DbContext
    {
        public BaseContext<KycCustomerEntity> Customers { get; set; }
        public BaseContext<KycCheckIdentityEntity> IdentityChecks { get; set; }
        public BaseContext<KycCheckAmlCtfEntity> AmlCtfChecks { get; set; }
        public KycDbContext(DbContextOptions<KycDbContext> options,
                BaseContext<KycCustomerEntity> customerContext,
                BaseContext<KycCheckIdentityEntity> identityContext,
                BaseContext<KycCheckAmlCtfEntity> amlCtfContext
            )
            : base(options)
        {
            Customers = customerContext;
            IdentityChecks = identityContext;
            AmlCtfChecks = amlCtfContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<KycCustomerEntity>().HasKey(c => c.Id);
            builder.Entity<KycCheckIdentityEntity>().HasKey(c => c.Id);
            builder.Entity<KycCheckAmlCtfEntity>().HasKey(c => c.Id);
        }
    }
}
