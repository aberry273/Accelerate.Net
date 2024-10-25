using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Rates.Migrations
{
    /// <inheritdoc />
    public partial class Rates_Migration_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RatesCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FundingSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatesCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RatesConversionOrderEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SellAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Memo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsAgreement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnBehalfOf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedSide = table.Column<int>(type: "int", nullable: false),
                    ConversionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConversionDatePreference = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatesConversionOrderEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatesConversionOrderEntity_RatesCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "RatesCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RatesConversionQuoteEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SellCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnBehalfOf = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedSide = table.Column<int>(type: "int", nullable: false),
                    ConversionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConversionDatePreference = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatesConversionQuoteEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatesConversionQuoteEntity_RatesCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "RatesCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RatesConversionOrderEntity_CustomerId",
                table: "RatesConversionOrderEntity",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RatesConversionQuoteEntity_CustomerId",
                table: "RatesConversionQuoteEntity",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatesConversionOrderEntity");

            migrationBuilder.DropTable(
                name: "RatesConversionQuoteEntity");

            migrationBuilder.DropTable(
                name: "RatesCustomer");
        }
    }
}
