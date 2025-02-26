using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderToMeeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Members",
                keyColumn: "UNFileNumber",
                keyValue: null,
                column: "UNFileNumber",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UNFileNumber",
                table: "Members",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Meetings",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Meetings");

            migrationBuilder.AlterColumn<string>(
                name: "UNFileNumber",
                table: "Members",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
