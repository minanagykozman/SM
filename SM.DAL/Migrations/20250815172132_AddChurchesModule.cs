using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChurchID",
                table: "Servants",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChurchID",
                table: "MemberFunds",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosureDate",
                table: "MemberFunds",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Churches",
                columns: table => new
                {
                    ChurchID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ChurchName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Churches", x => x.ChurchID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChurchMembers",
                columns: table => new
                {
                    ChurchID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChurchMembers", x => new { x.ChurchID, x.MemberID });
                    table.ForeignKey(
                        name: "FK_ChurchMembers_Churches_ChurchID",
                        column: x => x.ChurchID,
                        principalTable: "Churches",
                        principalColumn: "ChurchID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChurchMembers_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Servants",
                keyColumn: "ServantID",
                keyValue: -1,
                column: "ChurchID",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Servants_ChurchID",
                table: "Servants",
                column: "ChurchID");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFunds_ChurchID",
                table: "MemberFunds",
                column: "ChurchID");

            migrationBuilder.CreateIndex(
                name: "IX_ChurchMembers_MemberID",
                table: "ChurchMembers",
                column: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants");

            migrationBuilder.DropTable(
                name: "ChurchMembers");

            migrationBuilder.DropTable(
                name: "Churches");

            migrationBuilder.DropIndex(
                name: "IX_Servants_ChurchID",
                table: "Servants");

            migrationBuilder.DropIndex(
                name: "IX_MemberFunds_ChurchID",
                table: "MemberFunds");

            migrationBuilder.DropColumn(
                name: "ChurchID",
                table: "Servants");

            migrationBuilder.DropColumn(
                name: "ChurchID",
                table: "MemberFunds");

            migrationBuilder.DropColumn(
                name: "ClosureDate",
                table: "MemberFunds");
        }
    }
}
