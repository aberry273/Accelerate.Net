using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentPostLabel_ContentPosts_ContentPostId",
                table: "ContentPostLabel");

            migrationBuilder.DropIndex(
                name: "IX_ContentPostLabel_ContentPostId",
                table: "ContentPostLabel");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ContentPostLabel",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "ContentPostLabel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostLabel_ContentPostId",
                table: "ContentPostLabel",
                column: "ContentPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentPostLabel_ContentPosts_ContentPostId",
                table: "ContentPostLabel",
                column: "ContentPostId",
                principalTable: "ContentPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
