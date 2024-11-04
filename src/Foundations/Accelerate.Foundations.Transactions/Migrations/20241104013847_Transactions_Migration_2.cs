using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accelerate.Foundations.Transactions.Migrations
{
    /// <inheritdoc />
    public partial class Transactions_Migration_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionsCustomer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsCustomer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionsAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FundingSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionsAddress_TransactionsCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TransactionsCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionsTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnBehalfOf = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SourceTransferAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationTransferAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionsTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionsTransaction_TransactionsCustomer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "TransactionsCustomer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsAddress_CustomerId",
                table: "TransactionsAddress",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionsTransaction_CustomerId",
                table: "TransactionsTransaction",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionsAddress");

            migrationBuilder.DropTable(
                name: "TransactionsTransaction");

            migrationBuilder.DropTable(
                name: "TransactionsCustomer");
        }
    }
}
