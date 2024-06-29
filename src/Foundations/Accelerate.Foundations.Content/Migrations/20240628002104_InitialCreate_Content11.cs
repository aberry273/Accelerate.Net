using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quote",
                table: "ContentPostActions");

            migrationBuilder.RenameColumn(
                name: "Reply",
                table: "ContentPostActions",
                newName: "Like");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Like",
                table: "ContentPostActions",
                newName: "Reply");

            migrationBuilder.AddColumn<bool>(
                name: "Quote",
                table: "ContentPostActions",
                type: "bit",
                nullable: true);
        }
    }
}
