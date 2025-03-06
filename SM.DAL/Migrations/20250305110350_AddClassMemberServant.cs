using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassMemberServant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServantID",
                table: "ClassMembers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassMembers_ServantID",
                table: "ClassMembers",
                column: "ServantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassMembers_Servants_ServantID",
                table: "ClassMembers",
                column: "ServantID",
                principalTable: "Servants",
                principalColumn: "ServantID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassMembers_Servants_ServantID",
                table: "ClassMembers");

            migrationBuilder.DropIndex(
                name: "IX_ClassMembers_ServantID",
                table: "ClassMembers");

            migrationBuilder.DropColumn(
                name: "ServantID",
                table: "ClassMembers");
        }
    }
}
