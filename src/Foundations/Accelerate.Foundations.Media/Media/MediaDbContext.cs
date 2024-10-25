
using Accelerate.Foundations.Database.Services;
using Accelerate.Foundations.Media.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerator.Foundation.Media.Database
{
    public class MediaDbContext : DbContext
    {
        public BaseContext<MediaBlobEntity> Blobs { get; set; }
        public MediaDbContext(DbContextOptions<MediaDbContext> options,
                BaseContext<MediaBlobEntity> blobContext)
            : base(options)
        {
            Blobs = blobContext;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MediaBlobEntity>()
                .HasKey(c => c.Id);
        }
    }
}
