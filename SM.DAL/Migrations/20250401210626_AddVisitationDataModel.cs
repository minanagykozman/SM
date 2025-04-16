using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitationDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BaptismName",
                table: "Members",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Visitations",
                columns: table => new
                {
                    VisitationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    VisitationType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServantID = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedServantFeedback = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedServantID = table.Column<int>(type: "int", nullable: true),
                    Checked = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    VisitaionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitations", x => x.VisitationID);
                    table.ForeignKey(
                        name: "FK_Visitations_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visitations_Servants_AssignedServantID",
                        column: x => x.AssignedServantID,
                        principalTable: "Servants",
                        principalColumn: "ServantID");
                    table.ForeignKey(
                        name: "FK_Visitations_Servants_ServantID",
                        column: x => x.ServantID,
                        principalTable: "Servants",
                        principalColumn: "ServantID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_AssignedServantID",
                table: "Visitations",
                column: "AssignedServantID");

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_MemberID",
                table: "Visitations",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_Visitations_ServantID",
                table: "Visitations",
                column: "ServantID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visitations");

            migrationBuilder.DropColumn(
                name: "BaptismName",
                table: "Members");
        }
    }
}
