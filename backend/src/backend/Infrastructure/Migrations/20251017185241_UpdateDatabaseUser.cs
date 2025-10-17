using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.AlterColumn<string>(
                name: "NumberCard",
                table: "UserProfile",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                table: "UserProfile",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserProfile",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "AddressDetail",
                table: "UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProvinceCode",
                table: "UserProfile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProvinceName",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WardCode",
                table: "UserProfile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WardName",
                table: "UserProfile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "AddressDetail",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ProvinceCode",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "ProvinceName",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "WardCode",
                table: "UserProfile");

            migrationBuilder.DropColumn(
                name: "WardName",
                table: "UserProfile");

            migrationBuilder.AlterColumn<string>(
                name: "NumberCard",
                table: "UserProfile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                table: "UserProfile",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProfile",
                table: "UserProfile",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    AddressId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false),
                    DistrictCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WardCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    WardName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_UserAddress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_UserId",
                table: "UserAddress",
                column: "UserId");
        }
    }
}
