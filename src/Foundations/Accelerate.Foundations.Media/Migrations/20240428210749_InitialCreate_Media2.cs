using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Media.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Media2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentChannels",
                table: "ContentChannels");

            migrationBuilder.RenameTable(
                name: "ContentChannels",
                newName: "MediaBlobs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaBlobs",
                table: "MediaBlobs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaBlobs",
                table: "MediaBlobs");

            migrationBuilder.RenameTable(
                name: "MediaBlobs",
                newName: "ContentChannels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentChannels",
                table: "ContentChannels",
                column: "Id");
        }
    }
}
