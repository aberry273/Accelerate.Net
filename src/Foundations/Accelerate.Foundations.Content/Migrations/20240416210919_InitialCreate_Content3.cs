using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_QuoterContentPostId",
                table: "ContentPostQuotes");

            migrationBuilder.DropIndex(
                name: "IX_ContentPostQuotes_QuoterContentPostId",
                table: "ContentPostQuotes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContentPostQuotes_QuoterContentPostId",
                table: "ContentPostQuotes",
                column: "QuoterContentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPostQuotes_ContentPosts_QuoterContentPostId",
                table: "ContentPostQuotes",
                column: "QuoterContentPostId",
                principalTable: "ContentPosts",
                principalColumn: "Id");
        }
    }
}
