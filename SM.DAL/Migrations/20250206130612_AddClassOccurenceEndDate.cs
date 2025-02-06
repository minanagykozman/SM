using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassOccurenceEndDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClassOccurrenceDate",
                table: "ClassOccurrences",
                newName: "ClassOccurrenceStartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassOccurrenceEndDate",
                table: "ClassOccurrences",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassOccurrenceEndDate",
                table: "ClassOccurrences");

            migrationBuilder.RenameColumn(
                name: "ClassOccurrenceStartDate",
                table: "ClassOccurrences",
                newName: "ClassOccurrenceDate");
        }
    }
}
