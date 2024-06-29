using Accelerate.Foundations.Content.Models.Entities;
using Accelerate.Foundations.Database.Services;
using Elastic.Transport.Diagnostics.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Content.Database
{
    public class ContentDbContext : DbContext
    {
        public BaseContext<ContentPostMediaEntity> Media { get; set; }
        public BaseContext<ContentPostQuoteEntity> Quotes { get; set; }
        public BaseContext<ContentChannelEntity> Channels { get; set; }
        public BaseContext<ContentPostEntity> Posts { get; set; }
        public BaseContext<ContentPostSettingsEntity> Settings { get; set; }
        public BaseContext<ContentPostTaxonomyEntity> Taxonomys { get; set; }
        public ContentDbContext(DbContextOptions<ContentDbContext> options,
                BaseContext<ContentPostEntity> postContext,
                BaseContext<ContentPostMediaEntity> mediaContext,
                BaseContext<ContentPostQuoteEntity> quoteContext,
                BaseContext<ContentChannelEntity> channelContext,
                BaseContext<ContentPostSettingsEntity> settingsContext)
            : base(options)
        {
            Posts = postContext;
            Posts = postContext;
            Quotes = quoteContext;
            Media = mediaContext;
            Channels = channelContext;
            Settings = settingsContext;

            //base.SavingChanges += new EventHandler(context_SavingChanges);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ContentChannelEntity>().HasKey(c => c.Id);
            builder.Entity<ContentPostParentEntity>().HasKey(c => c.Id);
            builder.Entity<ContentPostChannelEntity>().HasKey(c => c.Id);
            builder.Entity<ContentPostMediaEntity>().HasKey(c => c.Id);
            builder.Entity<ContentPostSettingsEntity>().HasKey(c => c.Id);
            builder.Entity<ContentPostTaxonomyEntity>().HasKey(c => c.Id);

            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Activities)
                  .WithOne(x => x.ContentPost)
                  .HasForeignKey(x => x.ContentPostId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Actions)
                  .WithOne(x => x.ContentPost)
                  .HasForeignKey(x => x.ContentPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Quotes)
                  .WithOne(x => x.ContentPost)
                  .HasForeignKey(x => x.ContentPostId)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasOne(e => e.Summary)
                  .WithOne(x => x.ContentPost)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasOne(e => e.Link)
                  .WithOne(x => x.ContentPost)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasOne(e => e.Link)
                  .WithOne(x => x.ContentPost)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasOne(e => e.Taxonomy)
                  .WithOne(x => x.ContentPost)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostEntity>()
                .HasMany(e => e.Mentions)
                  .WithOne(x => x.ContentPost)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContentPostSettingsEntity>()
              .HasMany(e => e.ContentPosts)
              .WithOne(e => e.ContentPostSettings);
        }
    }
}
