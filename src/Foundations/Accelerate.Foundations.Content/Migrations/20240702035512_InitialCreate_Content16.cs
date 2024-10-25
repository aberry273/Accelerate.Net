using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reactions",
                table: "ContentPostActionsSummary",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TopReaction",
                table: "ContentPostActionsSummary",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reaction",
                table: "ContentPostActions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reactions",
                table: "ContentPostActionsSummary");

            migrationBuilder.DropColumn(
                name: "TopReaction",
                table: "ContentPostActionsSummary");

            migrationBuilder.DropColumn(
                name: "Reaction",
                table: "ContentPostActions");
        }
    }
}
