using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "ViewFunds" },
                    { 2, "ManageFunds" },
                    { 3, "DeleteUser" },
                    { 4, "ReviewFunds" }
                });

            migrationBuilder.InsertData(table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9924ae07-15dc-4893-a559-f65132952bd0", "FundAdmin","FUNDADMIN" },
                    { "1d3aec71-abee-4b98-a9e5-ba206c63be00", "ClassAdmin","CLASSADMIN" },
                    { "d3a75fca-e9e6-41cd-9060-5834d5918c51", "SuperAdmin","SUPPERADMIN" }
                });

            migrationBuilder.InsertData(table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { "9924ae07-15dc-4893-a559-f65132952bd0",2 }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Permissions",
                keyColumn: "Id",
                keyValues: new object[] { 1, 2, 3, 4 });
        }
    }
}
