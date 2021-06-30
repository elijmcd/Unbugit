using Microsoft.EntityFrameworkCore.Migrations;

namespace Unbugit.Data.Migrations
{
    public partial class _008 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectPriority_ProjectPriorityId",
                table: "Project");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectPriorityId",
                table: "Project",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectPriority_ProjectPriorityId",
                table: "Project",
                column: "ProjectPriorityId",
                principalTable: "ProjectPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_ProjectPriority_ProjectPriorityId",
                table: "Project");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectPriorityId",
                table: "Project",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_ProjectPriority_ProjectPriorityId",
                table: "Project",
                column: "ProjectPriorityId",
                principalTable: "ProjectPriority",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
