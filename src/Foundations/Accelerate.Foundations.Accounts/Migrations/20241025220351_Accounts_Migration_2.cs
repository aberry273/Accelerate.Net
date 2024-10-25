using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsFunding_AccountsCustomer_CustomerId",
                table: "AccountsFunding");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountsFunding",
                table: "AccountsFunding");

            migrationBuilder.RenameTable(
                name: "AccountsFunding",
                newName: "AccountsBankAccount");

            migrationBuilder.RenameIndex(
                name: "IX_AccountsFunding_CustomerId",
                table: "AccountsBankAccount",
                newName: "IX_AccountsBankAccount_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountsBankAccount",
                table: "AccountsBankAccount",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AccountsFundingSource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StreetAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Postcode = table.Column<int>(type: "int", nullable: false),
                    Suburb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsFundingSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsFundingSource_AccountsBankAccount_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "AccountsBankAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccountsFundingSource_AccountsCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AccountsCustomer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountsFundingSource_BankAccountId",
                table: "AccountsFundingSource",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountsFundingSource_CustomerId",
                table: "AccountsFundingSource",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsBankAccount_AccountsCustomer_CustomerId",
                table: "AccountsBankAccount",
                column: "CustomerId",
                principalTable: "AccountsCustomer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsBankAccount_AccountsCustomer_CustomerId",
                table: "AccountsBankAccount");

            migrationBuilder.DropTable(
                name: "AccountsFundingSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountsBankAccount",
                table: "AccountsBankAccount");

            migrationBuilder.RenameTable(
                name: "AccountsBankAccount",
                newName: "AccountsFunding");

            migrationBuilder.RenameIndex(
                name: "IX_AccountsBankAccount_CustomerId",
                table: "AccountsFunding",
                newName: "IX_AccountsFunding_CustomerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountsFunding",
                table: "AccountsFunding",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsFunding_AccountsCustomer_CustomerId",
                table: "AccountsFunding",
                column: "CustomerId",
                principalTable: "AccountsCustomer",
                principalColumn: "Id");
        }
    }
}
