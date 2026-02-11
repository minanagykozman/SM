using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustVisitationClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitations_Members_MemberID",
                table: "Visitations");

            migrationBuilder.DropIndex(
                name: "IX_Visitations_MemberID",
                table: "Visitations");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "Visitations",
                newName: "ClassID");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Visitations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VisitationMembers",
                columns: table => new
                {
                    VisitationID = table.Column<int>(type: "int", nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitationMembers", x => new { x.MemberID, x.VisitationID });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitationMembers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Visitations");

            migrationBuilder.RenameColumn(
                name: "ClassID",
                table: "Visitations",
                newName: "MemberID");

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
    }
}
