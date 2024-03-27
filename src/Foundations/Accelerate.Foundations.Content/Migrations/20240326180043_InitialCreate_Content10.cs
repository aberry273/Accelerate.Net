using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagItems",
                table: "ContentPosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagItems",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
