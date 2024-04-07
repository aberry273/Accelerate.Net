using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Content.Database
{
    public class ContentDbContext : DbContext
    {
        public BaseContext<ContentChannelEntity> Channels { get; set; }
        public BaseContext<ContentPostEntity> Posts { get; set; }
        public ContentDbContext(DbContextOptions<ContentDbContext> options,
                BaseContext<ContentPostEntity> postContext,
                BaseContext<ContentChannelEntity> channelContext)
            : base(options)
        {
            Posts = postContext;
            Channels = channelContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ContentChannelEntity>()
                .HasKey(c => c.Id);

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
