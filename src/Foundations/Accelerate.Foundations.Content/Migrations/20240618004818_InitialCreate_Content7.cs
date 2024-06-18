using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ContentPosts");

            migrationBuilder.CreateTable(
                name: "ContentPostMention",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostMention", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostMention_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostTaxonomy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostTaxonomy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostTaxonomy_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostMention_ContentPostId",
                table: "ContentPostMention",
                column: "ContentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostTaxonomy_ContentPostId",
                table: "ContentPostTaxonomy",
                column: "ContentPostId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentPostMention");

            migrationBuilder.DropTable(
                name: "ContentPostTaxonomy");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
