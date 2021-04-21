using Microsoft.EntityFrameworkCore.Migrations;

namespace StoreMVC.Data.Migrations
{
    public partial class changedkeynametoSalesPersonId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Appointments",
                newName: "SalesPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments",
                newName: "IX_Appointments_SalesPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_SalesPersonId",
                table: "Appointments",
                column: "SalesPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_SalesPersonId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "SalesPersonId",
                table: "Appointments",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_SalesPersonId",
                table: "Appointments",
                newName: "IX_Appointments_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
