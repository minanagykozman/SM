using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Classes",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AgeEndDate",
                table: "Classes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AgeStartDate",
                table: "Classes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChurchID",
                table: "Classes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassDay",
                table: "Classes",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassEndDate",
                table: "Classes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassEndTime",
                table: "Classes",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ClassFrequency",
                table: "Classes",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassStartDate",
                table: "Classes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassStartTime",
                table: "Classes",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Classes",
                type: "varchar(1)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Classes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ChurchID",
                table: "Classes",
                column: "ChurchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Churches_ChurchID",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ChurchID",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "AgeEndDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "AgeStartDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ChurchID",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassDay",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassEndDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassEndTime",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassFrequency",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassStartDate",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassStartTime",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Classes");

            migrationBuilder.AlterColumn<string>(
                name: "ClassName",
                table: "Classes",
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
