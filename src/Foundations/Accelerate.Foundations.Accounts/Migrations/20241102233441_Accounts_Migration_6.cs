using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_Migration_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "AccountsContact");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsIndividual",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsBusiness",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "AccountsBusiness",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsAccountContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsAccountChild",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "AccountsAccountAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsIndividual");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsContact");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsBusiness");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "AccountsBusiness");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsAddress");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsAccountContact");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsAccountChild");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "AccountsAccountAddress");

            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
