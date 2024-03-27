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
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "ContentPosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TagItems",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentPostActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostActivity_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContentPostReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Agree = table.Column<bool>(type: "bit", nullable: false),
                    Disagree = table.Column<bool>(type: "bit", nullable: false),
                    Like = table.Column<bool>(type: "bit", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostReview_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostActivity_ContentPostId",
                table: "ContentPostActivity",
                column: "ContentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostReview_ContentPostId",
                table: "ContentPostReview",
                column: "ContentPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentPostActivity");

            migrationBuilder.DropTable(
                name: "ContentPostReview");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "TagItems",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ContentPosts");
        }
    }
}
