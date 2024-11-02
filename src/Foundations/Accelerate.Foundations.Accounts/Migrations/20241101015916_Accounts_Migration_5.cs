using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_Migration_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsContact_AccountsAddress_AddressId",
                table: "AccountsContact");

            migrationBuilder.DropIndex(
                name: "IX_AccountsContact_AddressId",
                table: "AccountsContact");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "AccountsContact");

            migrationBuilder.DropColumn(
                name: "KycIdentityId",
                table: "AccountsContact");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AccountsContact");

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "AccountsBusiness",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Website",
                table: "AccountsBusiness");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "AccountsContact",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KycIdentityId",
                table: "AccountsContact",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AccountsContact",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AccountsContact_AddressId",
                table: "AccountsContact",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountsContact_AccountsAddress_AddressId",
                table: "AccountsContact",
                column: "AddressId",
                principalTable: "AccountsAddress",
                principalColumn: "Id");
        }
    }
}
