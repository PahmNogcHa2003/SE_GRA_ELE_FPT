using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserLifetimeStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLifetimeStats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    TotalDistanceKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTrips = table.Column<int>(type: "int", nullable: false),
                    TotalDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    TotalCo2SavedKg = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    TotalCaloriesBurned = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset(0)", precision: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLifetimeStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLifetimeStats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLifetimeStats_UserId",
                table: "UserLifetimeStats",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLifetimeStats");
        }
    }
}
