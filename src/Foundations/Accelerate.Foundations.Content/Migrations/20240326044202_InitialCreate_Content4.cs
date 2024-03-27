using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ContentPosts");

            migrationBuilder.AddColumn<string>(
                name: "Thread",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thread",
                table: "ContentPosts");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "ContentPosts",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
