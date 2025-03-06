using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAidClassModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AidClasses_Members_MemberID",
                table: "AidClasses");

            migrationBuilder.RenameColumn(
                name: "MemberID",
                table: "AidClasses",
                newName: "ClassID");

            migrationBuilder.RenameIndex(
                name: "IX_AidClasses_MemberID",
                table: "AidClasses",
                newName: "IX_AidClasses_ClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_AidClasses_Classes_ClassID",
                table: "AidClasses",
                column: "ClassID",
                principalTable: "Classes",
                principalColumn: "ClassID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AidClasses_Classes_ClassID",
                table: "AidClasses");

            migrationBuilder.RenameColumn(
                name: "ClassID",
                table: "AidClasses",
                newName: "MemberID");

            migrationBuilder.RenameIndex(
                name: "IX_AidClasses_ClassID",
                table: "AidClasses",
                newName: "IX_AidClasses_MemberID");

            migrationBuilder.AddForeignKey(
                name: "FK_AidClasses_Members_MemberID",
                table: "AidClasses",
                column: "MemberID",
                principalTable: "Members",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
