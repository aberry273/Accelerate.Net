using Accelerate.Foundations.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Accelerate.Foundations.Database.Services
{
    public class BaseContext<T> : DbContext where T : BaseEntity
    {
        public BaseContext(DbContextOptions<BaseContext<T>> options)
                : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlServer(options => options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null));
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Run OnCreate pipelines
        }

        protected void OnModelUpdating(ModelBuilder modelBuilder)
        {
            // Run OnUpdate pipelines
        }

        protected void OnModelDeleting(ModelBuilder modelBuilder)
        {
            // Run OnDelete pipelines
        }

        protected void OnModelGetting(ModelBuilder modelBuilder)
        {
            // Run OnGet pipelines
        }

        public virtual DbSet<T> Entities { get; set; }
    }
}
