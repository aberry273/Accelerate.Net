using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Content.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Content1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostChannel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaBlobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostMedia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostParent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostParent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostQuotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuotedContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostQuotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    threadId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    channelId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Access = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostLimit = table.Column<int>(type: "int", nullable: false),
                    CharLimit = table.Column<int>(type: "int", nullable: false),
                    ImageLimit = table.Column<int>(type: "int", nullable: false),
                    VideoLimit = table.Column<int>(type: "int", nullable: false),
                    Formats = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Agree = table.Column<bool>(type: "bit", nullable: true),
                    Disagree = table.Column<bool>(type: "bit", nullable: true),
                    Like = table.Column<bool>(type: "bit", nullable: true),
                    Reaction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostActions_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostActionsSummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Agrees = table.Column<int>(type: "int", nullable: false),
                    Disagrees = table.Column<int>(type: "int", nullable: false),
                    Quotes = table.Column<int>(type: "int", nullable: false),
                    Replies = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostActionsSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostActionsSummary_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostActivity_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostLink_ContentPosts_ContentPostId",
                        column: x => x.ContentPostId,
                        principalTable: "ContentPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPostSettingsPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentPostSettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPostSettingsPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentPostSettingsPost_ContentPostSettings_ContentPostSettingsId",
                        column: x => x.ContentPostSettingsId,
                        principalTable: "ContentPostSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostActions_ContentPostId",
                table: "ContentPostActions",
                column: "ContentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostActionsSummary_ContentPostId",
                table: "ContentPostActionsSummary",
                column: "ContentPostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostActivity_ContentPostId",
                table: "ContentPostActivity",
                column: "ContentPostId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostLink_ContentPostId",
                table: "ContentPostLink",
                column: "ContentPostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentPostSettingsPost_ContentPostSettingsId",
                table: "ContentPostSettingsPost",
                column: "ContentPostSettingsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentChannels");

            migrationBuilder.DropTable(
                name: "ContentPostActions");

            migrationBuilder.DropTable(
                name: "ContentPostActionsSummary");

            migrationBuilder.DropTable(
                name: "ContentPostActivity");

            migrationBuilder.DropTable(
                name: "ContentPostChannel");

            migrationBuilder.DropTable(
                name: "ContentPostLink");

            migrationBuilder.DropTable(
                name: "ContentPostMedia");

            migrationBuilder.DropTable(
                name: "ContentPostParent");

            migrationBuilder.DropTable(
                name: "ContentPostQuotes");

            migrationBuilder.DropTable(
                name: "ContentPostSettingsPost");

            migrationBuilder.DropTable(
                name: "ContentPosts");

            migrationBuilder.DropTable(
                name: "ContentPostSettings");
        }
    }
}
