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
                name: "Content",
                table: "ContentPostQuotes");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "ContentPostQuotes");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "ContentPostActions");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quote",
                table: "ContentPostActions");

            migrationBuilder.RenameColumn(
                name: "Reply",
                table: "ContentPostActions",
                newName: "Like");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "ContentPostQuotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "ContentPostQuotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "ContentPostActions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
