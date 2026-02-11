using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustVisitationCreationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Visitations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VisitationMembers_VisitationID",
                table: "VisitationMembers",
                column: "VisitationID");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitationMembers_Visitations_VisitationID",
                table: "VisitationMembers",
                column: "VisitationID",
                principalTable: "Visitations",
                principalColumn: "VisitationID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitationMembers_Visitations_VisitationID",
                table: "VisitationMembers");

            migrationBuilder.DropIndex(
                name: "IX_VisitationMembers_VisitationID",
                table: "VisitationMembers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Visitations");
        }
    }
}
