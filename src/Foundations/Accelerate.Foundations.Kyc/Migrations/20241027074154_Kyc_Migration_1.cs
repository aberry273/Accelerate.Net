using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Kyc.Migrations
{
    /// <inheritdoc />
    public partial class Kyc_Migration_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KycCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KycCheckIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KycCheckAmlCtfId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KycCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KycCheckAmlCtf",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalProvider = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KycCheckAmlCtf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KycCheckAmlCtf_KycCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "KycCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KycCheckIdentity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalProvider = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KycCheckIdentity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KycCheckIdentity_KycCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "KycCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KycCheckAmlCtf_CustomerId",
                table: "KycCheckAmlCtf",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_KycCheckIdentity_CustomerId",
                table: "KycCheckIdentity",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KycCheckAmlCtf");

            migrationBuilder.DropTable(
                name: "KycCheckIdentity");

            migrationBuilder.DropTable(
                name: "KycCustomer");
        }
    }
}
