using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MedicalAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 24, "Medical.View" },
                    { 25, "Medical.Manage" },
                    { 26, "Medical.Admin" }
                });

            migrationBuilder.InsertData(table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "f47ac10b-58cc-4372-a567-0e02b2c3d479", "MedicalManager", "MEDICALMANAGER" },
                    { "e2a9b3f0-4d51-4b89-8b01-527b1c2e8a6d", "MedicalAdmin", "MEDICALADMIN" }
                });

            migrationBuilder.InsertData(table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { "f47ac10b-58cc-4372-a567-0e02b2c3d479", 24 },
                    { "f47ac10b-58cc-4372-a567-0e02b2c3d479", 25 },
                    { "f47ac10b-58cc-4372-a567-0e02b2c3d479", 26 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
