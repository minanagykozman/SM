using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MakeApproverIDNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberFunds_Servants_ApproverID",
                table: "MemberFunds");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "MemberFunds",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ApproverID",
                table: "MemberFunds",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberFunds_Servants_ApproverID",
                table: "MemberFunds",
                column: "ApproverID",
                principalTable: "Servants",
                principalColumn: "ServantID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberFunds_Servants_ApproverID",
                table: "MemberFunds");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "MemberFunds",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "ApproverID",
                table: "MemberFunds",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberFunds_Servants_ApproverID",
                table: "MemberFunds",
                column: "ApproverID",
                principalTable: "Servants",
                principalColumn: "ServantID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
