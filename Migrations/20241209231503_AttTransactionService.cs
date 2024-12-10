using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_bank.Migrations
{
    /// <inheritdoc />
    public partial class AttTransactionService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverEmail",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "Transactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReceiverId",
                table: "Transactions",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_SenderId",
                table: "Transactions",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_ReceiverId",
                table: "Transactions",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_ReceiverId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_SenderId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReceiverId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_SenderId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "SenderId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverEmail",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Transactions",
                type: "text",
                nullable: true);
        }
    }
}
