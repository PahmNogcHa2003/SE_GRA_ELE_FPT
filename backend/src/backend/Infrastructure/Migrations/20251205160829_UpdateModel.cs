using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherUsage_Order_OrderId",
                table: "VoucherUsage");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "VoucherUsage",
                newName: "TicketPlanPriceId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherUsage_OrderId",
                table: "VoucherUsage",
                newName: "IX_VoucherUsage_TicketPlanPriceId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Voucher",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherUsage_TicketPlanPrice_TicketPlanPriceId",
                table: "VoucherUsage",
                column: "TicketPlanPriceId",
                principalTable: "TicketPlanPrice",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VoucherUsage_TicketPlanPrice_TicketPlanPriceId",
                table: "VoucherUsage");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Voucher");

            migrationBuilder.RenameColumn(
                name: "TicketPlanPriceId",
                table: "VoucherUsage",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_VoucherUsage_TicketPlanPriceId",
                table: "VoucherUsage",
                newName: "IX_VoucherUsage_OrderId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Voucher",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherUsage_Order_OrderId",
                table: "VoucherUsage",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id");
        }
    }
}
