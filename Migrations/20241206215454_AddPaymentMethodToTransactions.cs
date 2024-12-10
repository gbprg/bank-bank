using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transfer_bank.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodToTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Transactions");

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
        }
    }
}
