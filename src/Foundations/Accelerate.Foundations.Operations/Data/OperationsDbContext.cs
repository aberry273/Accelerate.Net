using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Operations.Models;
using Accelerate.Foundations.Operations.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Content.Database
{
    public class OperationsDbContext : DbContext
    {
        public BaseContext<OperationsJobEntity> Jobs { get; set; }
        public BaseContext<OperationsActionEntity> Actions { get; set; }
        public BaseContext<OperationsActivityEntity> Activities { get; set; }
        public OperationsDbContext(DbContextOptions<OperationsDbContext> options,
                BaseContext<OperationsJobEntity> jobContext,
                BaseContext<OperationsActionEntity> actionContext,
                BaseContext<OperationsActivityEntity> activityContext)
            : base(options)
        {
            Jobs = jobContext;
            Actions = actionContext;
            Activities = activityContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<OperationsJobEntity>().HasKey(c => c.Id);
            builder.Entity<OperationsActionEntity>().HasKey(c => c.Id);
            builder.Entity<OperationsActivityEntity>().HasKey(c => c.Id);
        }
    }
}
