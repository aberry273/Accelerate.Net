using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_Migration_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsAccountChild_AccountsAccount_AccountsAccountId",
                table: "AccountsAccountChild");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountsAccountContact_AccountsAccount_AccountsAccountId",
                table: "AccountsAccountContact");

            migrationBuilder.DropTable(
                name: "AccountsAccount");

            migrationBuilder.CreateTable(
                name: "AccountsBusiness",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OperatingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrimaryContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SignedAgreementId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsBusiness", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountsIndividual",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KycIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsIndividual", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountsIndividual_AccountsAddress_AddressId",
                        column: x => x.AddressId,
                        principalTable: "AccountsAddress",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountsIndividual_AddressId",
                table: "AccountsIndividual",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsAccountChild_AccountsBusiness_AccountsAccountId",
                table: "AccountsAccountChild",
                column: "AccountsAccountId",
                principalTable: "AccountsBusiness",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsAccountContact_AccountsBusiness_AccountsAccountId",
                table: "AccountsAccountContact",
                column: "AccountsAccountId",
                principalTable: "AccountsBusiness",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsAccountChild_AccountsBusiness_AccountsAccountId",
                table: "AccountsAccountChild");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountsAccountContact_AccountsBusiness_AccountsAccountId",
                table: "AccountsAccountContact");

            migrationBuilder.DropTable(
                name: "AccountsBusiness");

            migrationBuilder.DropTable(
                name: "AccountsIndividual");

            migrationBuilder.CreateTable(
                name: "AccountsAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperatingAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegisteredAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignedAgreementId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsAccount", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsAccountChild_AccountsAccount_AccountsAccountId",
                table: "AccountsAccountChild",
                column: "AccountsAccountId",
                principalTable: "AccountsAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsAccountContact_AccountsAccount_AccountsAccountId",
                table: "AccountsAccountContact",
                column: "AccountsAccountId",
                principalTable: "AccountsAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
