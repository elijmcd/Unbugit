using Microsoft.EntityFrameworkCore.Migrations;

namespace Unbugit.Data.Migrations
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistory_AspNetUsers_UserId1",
                table: "TicketHistory");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistory_UserId1",
                table: "TicketHistory");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "TicketHistory");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TicketHistory",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_UserId",
                table: "TicketHistory",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistory_AspNetUsers_UserId",
                table: "TicketHistory",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistory_AspNetUsers_UserId",
                table: "TicketHistory");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistory_UserId",
                table: "TicketHistory");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TicketHistory",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "TicketHistory",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_UserId1",
                table: "TicketHistory",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistory_AspNetUsers_UserId1",
                table: "TicketHistory",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
