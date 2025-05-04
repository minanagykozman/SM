using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustMemberFunds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Funds");

            migrationBuilder.DropColumn(
                name: "Delivered",
                table: "MemberFunds");

            migrationBuilder.RenameColumn(
                name: "ApprovedDescription",
                table: "MemberFunds",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "ApproverNotes",
                table: "MemberFunds",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "MemberFunds",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproverNotes",
                table: "MemberFunds");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "MemberFunds");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "MemberFunds",
                newName: "ApprovedDescription");

            migrationBuilder.AddColumn<bool>(
                name: "Delivered",
                table: "MemberFunds",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    FundID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AidID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    ServantID = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.FundID);
                    table.ForeignKey(
                        name: "FK_Funds_Aids_AidID",
                        column: x => x.AidID,
                        principalTable: "Aids",
                        principalColumn: "AidID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Funds_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Funds_Servants_ServantID",
                        column: x => x.ServantID,
                        principalTable: "Servants",
                        principalColumn: "ServantID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_AidID",
                table: "Funds",
                column: "AidID");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_MemberID",
                table: "Funds",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Funds_ServantID",
                table: "Funds",
                column: "ServantID");
        }
    }
}
