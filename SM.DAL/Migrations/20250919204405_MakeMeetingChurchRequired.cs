using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeMeetingChurchRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Meetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Churches_ChurchID",
                table: "Meetings",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");
        }
    }
}
