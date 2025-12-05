using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelRentalandStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rental_EndStationId",
                table: "Rental",
                column: "EndStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rental_StartStationId",
                table: "Rental",
                column: "StartStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Station_EndStationId",
                table: "Rental",
                column: "EndStationId",
                principalTable: "Station",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rental_Station_StartStationId",
                table: "Rental",
                column: "StartStationId",
                principalTable: "Station",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Station_EndStationId",
                table: "Rental");

            migrationBuilder.DropForeignKey(
                name: "FK_Rental_Station_StartStationId",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Rental_EndStationId",
                table: "Rental");

            migrationBuilder.DropIndex(
                name: "IX_Rental_StartStationId",
                table: "Rental");
        }
    }
}
