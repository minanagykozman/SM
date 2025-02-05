using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "30f10ff2-8ac3-4acc-b88d-abb2fd554653", null, "Servant", "SERVANT" },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "383d20f8-ac3b-46ff-b322-4f21cda036dc", "78769c53-5336-4411-8488-1983111d7be7" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "78769c53-5336-4411-8488-1983111d7be7", 0, "7c62d744-b320-49c6-871a-273d2f53a81f", "ssudan.stpual@gmail.com", true, false, null, "SSUDAN.STPAUL@GMAIL.COM", "SSUDAN.STPAUL@GMAIL.COM", "AQAAAAIAAYagAAAAEAvxm8Ul6/CAsvy/Ylk9GobRdQrfCnhyTSTEO0s149pYHw6oPn9vcCqhwIvF558hSw==", null, false, "394139b6-fd9c-4d25-a9a5-ef1565d13bab", false, "ssudan.stpual@gmail.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "30f10ff2-8ac3-4acc-b88d-abb2fd554653");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: "383d20f8-ac3b-46ff-b322-4f21cda036dc");

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "383d20f8-ac3b-46ff-b322-4f21cda036dc", "78769c53-5336-4411-8488-1983111d7be7" });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "78769c53-5336-4411-8488-1983111d7be7");
        }
    }
}
