using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "ContentPosts",
                newName: "Text");

            migrationBuilder.AddColumn<string>(
                name: "FormatsValue",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormatsValue",
                table: "ContentPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ContentPosts");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "ContentPosts",
                newName: "Content");
        }
    }
}
