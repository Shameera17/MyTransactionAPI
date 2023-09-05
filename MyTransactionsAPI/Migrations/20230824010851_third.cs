using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTransactionsAPI.Migrations
{
    /// <inheritdoc />
    public partial class third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransactionTypeId1",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionTypeId1",
                table: "Transactions",
                column: "TransactionTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionTypeId1",
                table: "Transactions",
                column: "TransactionTypeId1",
                principalTable: "TransactionTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionTypes_TransactionTypeId1",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionTypeId1",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionTypeId1",
                table: "Transactions");
        }
    }
}
