using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPostActivity_ContentPosts_ContentPostId",
                table: "ContentPostActivity");

            migrationBuilder.DropIndex(
                name: "IX_ContentPostActivity_ContentPostId",
                table: "ContentPostActivity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContentPostActivity_ContentPostId",
                table: "ContentPostActivity",
                column: "ContentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPostActivity_ContentPosts_ContentPostId",
                table: "ContentPostActivity",
                column: "ContentPostId",
                principalTable: "ContentPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
