﻿// <auto-generated />
using System;
using Accelerator.Foundation.Content.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    [DbContext(typeof(ContentDbContext))]
    [Migration("20240618004818_InitialCreate_Content7")]
    partial class InitialCreate_Content7
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentChannelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ContentChannels");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActionsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("Agree")
                        .HasColumnType("bit");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("Disagree")
                        .HasColumnType("bit");

                    b.Property<bool?>("Like")
                        .HasColumnType("bit");

                    b.Property<string>("Reaction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostActions");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActionsSummaryEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Agrees")
                        .HasColumnType("int");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Disagrees")
                        .HasColumnType("int");

                    b.Property<int>("Quotes")
                        .HasColumnType("int");

                    b.Property<int>("Replies")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId")
                        .IsUnique();

                    b.ToTable("ContentPostActionsSummary");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActivityEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostActivity");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostChannelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ChannelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ContentPostChannel");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ContentPosts");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostLinkEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId")
                        .IsUnique();

                    b.ToTable("ContentPostLink");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostMediaEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("MediaBlobId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ContentPostMedia");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostMentionEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostMention");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostParentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ParentIds")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("ContentPostParent");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostQuoteEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("QuotedContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Response")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId");

                    b.ToTable("ContentPostQuotes");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostSettingsEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Access")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CharLimit")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Formats")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ImageLimit")
                        .HasColumnType("int");

                    b.Property<int>("PostLimit")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("VideoLimit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("ContentPostSettings");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostSettingsPostEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContentPostSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostSettingsId");

                    b.ToTable("ContentPostSettingsPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostTaxonomyEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ContentPostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ContentPostId")
                        .IsUnique();

                    b.ToTable("ContentPostTaxonomy");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActionsEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithMany("Actions")
                        .HasForeignKey("ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActionsSummaryEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithOne("Summary")
                        .HasForeignKey("Accelerate.Foundations.Content.Models.Entities.ContentPostActionsSummaryEntity", "ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostActivityEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithMany("Activities")
                        .HasForeignKey("ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostLinkEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithOne("Link")
                        .HasForeignKey("Accelerate.Foundations.Content.Models.Entities.ContentPostLinkEntity", "ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostMentionEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithMany("Mentions")
                        .HasForeignKey("ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostQuoteEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithMany("Quotes")
                        .HasForeignKey("ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostSettingsPostEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostSettingsEntity", "ContentPostSettings")
                        .WithMany("ContentPosts")
                        .HasForeignKey("ContentPostSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPostSettings");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostTaxonomyEntity", b =>
                {
                    b.HasOne("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", "ContentPost")
                        .WithOne("Taxonomy")
                        .HasForeignKey("Accelerate.Foundations.Content.Models.Entities.ContentPostTaxonomyEntity", "ContentPostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ContentPost");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostEntity", b =>
                {
                    b.Navigation("Actions");

                    b.Navigation("Activities");

                    b.Navigation("Link");

                    b.Navigation("Mentions");

                    b.Navigation("Quotes");

                    b.Navigation("Summary");

                    b.Navigation("Taxonomy");
                });

            modelBuilder.Entity("Accelerate.Foundations.Content.Models.Entities.ContentPostSettingsEntity", b =>
                {
                    b.Navigation("ContentPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
