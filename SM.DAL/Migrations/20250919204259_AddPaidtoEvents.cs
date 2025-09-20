using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPaidtoEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChurchID",
                table: "Meetings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Paid",
                table: "EventRegistrations",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ChurchID",
                table: "Meetings",
                column: "ChurchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");

            migrationBuilder.Sql(@"
                UPDATE Meetings SET ChurchID = 1
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_ChurchID",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "ChurchID",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "EventRegistrations");
        }
    }
}
