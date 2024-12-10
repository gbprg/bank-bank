using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_bank.Migrations
{
    /// <inheritdoc />
    public partial class rollbackTransactionService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "Transactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
