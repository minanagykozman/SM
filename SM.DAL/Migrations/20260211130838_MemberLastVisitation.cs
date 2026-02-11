using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MemberLastVisitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastVisitationType",
                table: "Members",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisitationType",
                table: "Members");
        }
    }
}
