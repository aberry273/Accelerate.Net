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
            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "ContentPosts",
                newName: "TargetThread");

            migrationBuilder.RenameColumn(
                name: "ChannelId",
                table: "ContentPosts",
                newName: "TargetChannel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TargetThread",
                table: "ContentPosts",
                newName: "ThreadId");

            migrationBuilder.RenameColumn(
                name: "TargetChannel",
                table: "ContentPosts",
                newName: "ChannelId");
        }
    }
}
