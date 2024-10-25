using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "ParentIds",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "channelId",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "threadId",
                table: "ContentPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "ContentPosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentIds",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "channelId",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "threadId",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
