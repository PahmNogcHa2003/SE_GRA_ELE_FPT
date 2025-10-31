using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketPlanAndUserTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "TicketPlanPrice");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "TicketPlanPrice");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActivationDeadline",
                table: "UserTicket",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "UserTicket",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "UserTicket",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivationMode",
                table: "TicketPlanPrice",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ActivationWindowDays",
                table: "TicketPlanPrice",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationDeadline",
                table: "UserTicket");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "UserTicket");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "UserTicket");

            migrationBuilder.DropColumn(
                name: "ActivationMode",
                table: "TicketPlanPrice");

            migrationBuilder.DropColumn(
                name: "ActivationWindowDays",
                table: "TicketPlanPrice");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidFrom",
                table: "TicketPlanPrice",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ValidTo",
                table: "TicketPlanPrice",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);
        }
    }
}
