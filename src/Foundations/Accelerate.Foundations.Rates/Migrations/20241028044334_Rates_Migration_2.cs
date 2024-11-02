using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Rates.Migrations
{
    /// <inheritdoc />
    public partial class Rates_Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FundingSourceId",
                table: "RatesCustomer",
                newName: "AccountsAccountId");

            migrationBuilder.RenameColumn(
                name: "SellCurrency",
                table: "RatesConversionQuoteEntity",
                newName: "SellAsset");

            migrationBuilder.RenameColumn(
                name: "BuyCurrency",
                table: "RatesConversionQuoteEntity",
                newName: "ExternalId");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "RatesConversionQuoteEntity",
                newName: "Volume");

            migrationBuilder.RenameColumn(
                name: "SellCurrency",
                table: "RatesConversionOrderEntity",
                newName: "SellAsset");

            migrationBuilder.RenameColumn(
                name: "BuyCurrency",
                table: "RatesConversionOrderEntity",
                newName: "ExternalId");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "RatesConversionOrderEntity",
                newName: "Volume");

            migrationBuilder.AddColumn<string>(
                name: "BuyAsset",
                table: "RatesConversionQuoteEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "RatesConversionQuoteEntity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "RatesConversionQuoteEntity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "RatesConversionQuoteEntity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BuyAsset",
                table: "RatesConversionOrderEntity",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "RatesConversionOrderEntity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "RatesConversionOrderEntity",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "RatesConversionOrderEntity",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyAsset",
                table: "RatesConversionQuoteEntity");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "RatesConversionQuoteEntity");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "RatesConversionQuoteEntity");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "RatesConversionQuoteEntity");

            migrationBuilder.DropColumn(
                name: "BuyAsset",
                table: "RatesConversionOrderEntity");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "RatesConversionOrderEntity");

            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "RatesConversionOrderEntity");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RatesConversionOrderEntity");

            migrationBuilder.RenameColumn(
                name: "AccountsAccountId",
                table: "RatesCustomer",
                newName: "FundingSourceId");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "RatesConversionQuoteEntity",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "SellAsset",
                table: "RatesConversionQuoteEntity",
                newName: "SellCurrency");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "RatesConversionQuoteEntity",
                newName: "BuyCurrency");

            migrationBuilder.RenameColumn(
                name: "Volume",
                table: "RatesConversionOrderEntity",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "SellAsset",
                table: "RatesConversionOrderEntity",
                newName: "SellCurrency");

            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "RatesConversionOrderEntity",
                newName: "BuyCurrency");
        }
    }
}
