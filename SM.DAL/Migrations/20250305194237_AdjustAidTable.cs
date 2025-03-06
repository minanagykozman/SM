using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustAidTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventStartDate",
                table: "Aids",
                newName: "AidDate");

            migrationBuilder.AlterColumn<string>(
                name: "AidName",
                table: "Aids",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ActualMembersCount",
                table: "Aids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Components",
                table: "Aids",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPerPerson",
                table: "Aids",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PlannedMembersCount",
                table: "Aids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Aids",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualMembersCount",
                table: "Aids");

            migrationBuilder.DropColumn(
                name: "Components",
                table: "Aids");

            migrationBuilder.DropColumn(
                name: "CostPerPerson",
                table: "Aids");

            migrationBuilder.DropColumn(
                name: "PlannedMembersCount",
                table: "Aids");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Aids");

            migrationBuilder.RenameColumn(
                name: "AidDate",
                table: "Aids",
                newName: "EventStartDate");

            migrationBuilder.AlterColumn<string>(
                name: "AidName",
                table: "Aids",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
