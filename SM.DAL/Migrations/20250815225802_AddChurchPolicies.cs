using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddChurchPolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Servants",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "MemberFunds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.InsertData(table: "Permissions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 21, "Church.ViewAll" },
                    { 22, "Church.EditAll" },
                    { 23, "Church.ManageAll" }
                });

            migrationBuilder.InsertData(table: "Roles",
                columns: new[] { "Id", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7c1e54a2-fb0d-4baf-9e21-8a42b8735a17", "AllChurchViewer", "ALLCHURCHVIEWER" },
                    { "d9f7b0c3-6e28-4a97-bf15-3a26f0f1cd9e", "AllChurchEditor", "ALLCHURCHEDITOR" }
                });

            migrationBuilder.InsertData(table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                values: new object[,]
                {
                    { "7c1e54a2-fb0d-4baf-9e21-8a42b8735a17", 21 },
                    { "d9f7b0c3-6e28-4a97-bf15-3a26f0f1cd9e", 22 },
                    { "383d20f8-ac3b-46ff-b322-4f21cda036dc", 23 }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "Servants",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ChurchID",
                table: "MemberFunds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "Servants",
                keyColumn: "ServantID",
                keyValue: -1,
                column: "ChurchID",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberFunds_Churches_ChurchID",
                table: "MemberFunds",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Servants_Churches_ChurchID",
                table: "Servants",
                column: "ChurchID",
                principalTable: "Churches",
                principalColumn: "ChurchID");
        }
    }
}
