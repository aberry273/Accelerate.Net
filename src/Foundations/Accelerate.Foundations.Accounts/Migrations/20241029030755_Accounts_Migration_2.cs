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
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AccountNumberType",
                table: "AccountsBankAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountsContact_AccountsAddress_AddressId",
                table: "AccountsContact");

            migrationBuilder.DropIndex(
                name: "IX_AccountsContact_AddressId",
                table: "AccountsContact");

            migrationBuilder.DropColumn(
                name: "AccountNumberType",
                table: "AccountsBankAccount");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "AccountsContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
