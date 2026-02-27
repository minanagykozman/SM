using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ClassChurchIDMandatory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Classes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");
        }
    }
}
