using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminServant_RemoveRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Servants");

            migrationBuilder.InsertData(
                table: "Servants",
                columns: new[] { "ServantID", "IsActive", "Mobile1", "Mobile2", "ServantName", "UserID" },
                values: new object[] { -1, true, "", null, "admin", "78769c53-5336-4411-8488-1983111d7be7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Servants",
                keyColumn: "ServantID",
                keyValue: -1);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Servants",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
