using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddmodelVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VoucherId",
                table: "TicketPlanPrice",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsPercentage = table.Column<bool>(type: "bit", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsagePerUser = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherUsage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: true),
                    UsedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherUsage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherUsage_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherUsage_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VoucherUsage_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketPlanPrice_VoucherId",
                table: "TicketPlanPrice",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUsage_OrderId",
                table: "VoucherUsage",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUsage_UserId",
                table: "VoucherUsage",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUsage_VoucherId",
                table: "VoucherUsage",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketPlanPrice_Voucher_VoucherId",
                table: "TicketPlanPrice",
                column: "VoucherId",
                principalTable: "Voucher",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketPlanPrice_Voucher_VoucherId",
                table: "TicketPlanPrice");

            migrationBuilder.DropTable(
                name: "VoucherUsage");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropIndex(
                name: "IX_TicketPlanPrice_VoucherId",
                table: "TicketPlanPrice");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "TicketPlanPrice");
        }
    }
}
