using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMeetings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Meetings_MeetingID",
                table: "Classes");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Classes_MeetingID",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "MeetingID",
                table: "Classes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeetingID",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    MeetingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChurchID = table.Column<int>(type: "int", nullable: false),
                    AgeEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AgeStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Gender = table.Column<string>(type: "varchar(1)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MeetingDay = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeetingEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MeetingEndTime = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeetingFrequency = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeetingName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeetingStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    MeetingStartTime = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.MeetingID);
                    table.ForeignKey(
                        name: "FK_Meetings_Churches_ChurchID",
                        column: x => x.ChurchID,
                        principalTable: "Churches",
                        principalColumn: "ChurchID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_MeetingID",
                table: "Classes",
                column: "MeetingID");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ChurchID",
                table: "Meetings",
                column: "ChurchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Meetings_MeetingID",
                table: "Classes",
                column: "MeetingID",
                principalTable: "Meetings",
                principalColumn: "MeetingID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
