using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_AspNetUsers_AssignedTo",
                table: "Contact");

            migrationBuilder.DropForeignKey(
                name: "FK_Contact_AspNetUsers_UserId",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_AssignedTo",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Contact",
                newName: "ReplyById");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_UserId",
                table: "Contact",
                newName: "IX_Contact_ReplyById");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_AspNetUsers_ReplyById",
                table: "Contact",
                column: "ReplyById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_AspNetUsers_ReplyById",
                table: "Contact");

            migrationBuilder.RenameColumn(
                name: "ReplyById",
                table: "Contact",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_ReplyById",
                table: "Contact",
                newName: "IX_Contact_UserId");

            migrationBuilder.AddColumn<long>(
                name: "AssignedTo",
                table: "Contact",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_AssignedTo",
                table: "Contact",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_AspNetUsers_AssignedTo",
                table: "Contact",
                column: "AssignedTo",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_AspNetUsers_UserId",
                table: "Contact",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
