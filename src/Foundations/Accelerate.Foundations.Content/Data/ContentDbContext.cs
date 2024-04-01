using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Finance.Database
{
    public class ContentDbContext : DbContext
    {
        public BaseContext<ContentPostEntity> Posts { get; set; }
        public ContentDbContext(DbContextOptions<ContentDbContext> options,
                BaseContext<ContentPostEntity> postContext)
            : base(options)
        {
            Posts = postContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Activities)
                  .WithOne(x => x.ContentPost)
                  .HasForeignKey(x => x.ContentPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Reviews)
                  .WithOne(x => x.ContentPost)
                  .HasForeignKey(x => x.ContentPostId)
                  .OnDelete(DeleteBehavior.Cascade);
            /*
            // Activities 
            builder.Entity<ContentPostEntity>()
              .HasMany(e => e.Activities)
              .WithOne(x => x.ContentPost)
              .HasForeignKey(x => x.ContentPostId)
              .OnDelete(DeleteBehavior.Cascade);

            // Reviews
            builder.Entity<ContentPostEntity>()
              .HasMany(e => e.Reviews)
              .WithOne(x => x.ContentPost)
              .HasForeignKey(x => x.ContentPostId)
              .OnDelete(DeleteBehavior.Cascade);
            */
        }
    }
}
