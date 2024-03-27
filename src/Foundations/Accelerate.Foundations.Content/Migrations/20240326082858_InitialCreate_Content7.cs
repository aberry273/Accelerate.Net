using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Thread",
                table: "ContentPosts",
                newName: "ThreadId");

            migrationBuilder.AddColumn<string>(
                name: "ChannelId",
                table: "ContentPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "ContentPosts");

            migrationBuilder.RenameColumn(
                name: "ThreadId",
                table: "ContentPosts",
                newName: "Thread");
        }
    }
}
