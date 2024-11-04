using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Accounts.Migrations
{
    /// <inheritdoc />
    public partial class Accounts_Migration_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "AccountsBusiness",
                newName: "AccountType");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "AccountsIndividual",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Postcode",
                table: "AccountsAddress",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "AccountsIndividual");

            migrationBuilder.RenameColumn(
                name: "AccountType",
                table: "AccountsBusiness",
                newName: "Type");

            migrationBuilder.AlterColumn<int>(
                name: "Postcode",
                table: "AccountsAddress",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
