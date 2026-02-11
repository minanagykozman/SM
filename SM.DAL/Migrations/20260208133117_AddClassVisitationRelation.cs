using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddClassVisitationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Visitations_ClassID",
                table: "Visitations",
                column: "ClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_Visitations_Classes_ClassID",
                table: "Visitations",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visitations_Classes_ClassID",
                table: "Visitations");

            migrationBuilder.DropIndex(
                name: "IX_Visitations_ClassID",
                table: "Visitations");
        }
    }
}
