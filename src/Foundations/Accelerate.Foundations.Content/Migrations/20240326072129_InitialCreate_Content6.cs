using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Channel",
                table: "ContentPosts",
                newName: "Category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "ContentPosts",
                newName: "Channel");
        }
    }
}
