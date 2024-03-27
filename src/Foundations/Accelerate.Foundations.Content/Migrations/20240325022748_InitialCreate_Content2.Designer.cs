﻿// <auto-generated />
using System;
using Accelerator.Foundation.Finance.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    [DbContext(typeof(ContentDbContext))]
    [Migration("20240325022748_InitialCreate_Content2")]
    partial class InitialCreate_Content2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostActivityEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostActivity");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagItems")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ContentPosts");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostReviewEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Agree")
                        .HasColumnType("bit");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<bool>("Disagree")
                        .HasColumnType("bit");

                    b.Property<bool>("Like")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedOn")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostReview");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostActivityEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.ContentPostEntity", "ContentPost")
                        .WithMany("Activities")
                        .HasForeignKey("ContentPostId");

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostReviewEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.ContentPostEntity", "ContentPost")
                        .WithMany("Reviews")
                        .HasForeignKey("ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.ContentPostEntity", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Reviews");
                });
#pragma warning restore 612, 618
        }
    }
}
