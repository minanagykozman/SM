using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustVisitationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitationMembers");

            migrationBuilder.AddColumn<int>(
                name: "MemberID",
                table: "Visitations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_MemberID",
                table: "Visitations",
                column: "MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitations_Members_MemberID",
                table: "Visitations",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitations_Members_MemberID",
                table: "Visitations");

            migrationBuilder.DropIndex(
                name: "IX_Visitations_MemberID",
                table: "Visitations");

            migrationBuilder.DropColumn(
                name: "MemberID",
                table: "Visitations");

            migrationBuilder.CreateTable(
                name: "VisitationMembers",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    VisitationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitationMembers", x => new { x.MemberID, x.VisitationID });
                    table.ForeignKey(
                        name: "FK_VisitationMembers_Visitations_VisitationID",
                        column: x => x.VisitationID,
                        principalTable: "Visitations",
                        principalColumn: "VisitationID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VisitationMembers_VisitationID",
                table: "VisitationMembers",
                column: "VisitationID");
        }
    }
}
