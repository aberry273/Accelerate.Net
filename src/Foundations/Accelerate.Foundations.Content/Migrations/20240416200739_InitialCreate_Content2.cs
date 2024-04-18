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
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_QuotedContentPostId",
                table: "ContentPostQuotes");

            migrationBuilder.DropIndex(
                name: "IX_ContentPostQuotes_QuotedContentPostId",
                table: "ContentPostQuotes");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ContentPostQuotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "ContentPostQuotes");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostQuotes_QuotedContentPostId",
                table: "ContentPostQuotes",
                column: "QuotedContentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_QuotedContentPostId",
                table: "ContentPostQuotes",
                column: "QuotedContentPostId",
                principalTable: "ContentPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
