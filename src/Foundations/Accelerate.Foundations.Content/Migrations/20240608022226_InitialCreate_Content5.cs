using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContentPostQuotes_ContentPostId",
                table: "ContentPostQuotes",
                column: "ContentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_ContentPostId",
                table: "ContentPostQuotes",
                column: "ContentPostId",
                principalTable: "ContentPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_ContentPostId",
                table: "ContentPostQuotes");

            migrationBuilder.DropIndex(
                name: "IX_ContentPostQuotes_ContentPostId",
                table: "ContentPostQuotes");
        }
    }
}
