using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "Dob",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "IssuedBy",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "IssuedDate",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "PlaceOfOrigin",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "PlaceOfResidence",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "SelfieUrl",
                table: "KycForm");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "KycForm");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "KycForm",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTime>(
                name: "Dob",
                table: "KycForm",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpiryDate",
                table: "KycForm",
                type: "datetimeoffset(7)",
                precision: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "KycForm",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "KycForm",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IssuedBy",
                table: "KycForm",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "IssuedDate",
                table: "KycForm",
                type: "datetimeoffset(7)",
                precision: 7,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfOrigin",
                table: "KycForm",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceOfResidence",
                table: "KycForm",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "KycForm",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewedAt",
                table: "KycForm",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelfieUrl",
                table: "KycForm",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "KycForm",
                type: "datetimeoffset(0)",
                precision: 0,
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
