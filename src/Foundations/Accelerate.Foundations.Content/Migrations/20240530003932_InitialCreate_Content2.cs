using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "ContentPostQuotes");

            migrationBuilder.RenameColumn(
                name: "QuoterContentPostId",
                table: "ContentPostQuotes",
                newName: "ContentPostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentPostId",
                table: "ContentPostQuotes",
                newName: "QuoterContentPostId");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "ContentPostQuotes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
