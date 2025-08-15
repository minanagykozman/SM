using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultChurchValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(table: "Churches",
               columns: new[] { "ChurchID", "ChurchName", "Address" },
               values: new object[,]
               {
                    { 1, "St Mary & Arch Michael - Ain Shams", "Ahmed Essmat" }
               });

            migrationBuilder.Sql(@"
                INSERT INTO ChurchMembers (ChurchID, MemberID)
                SELECT 1, m.MemberID
                FROM Members m
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM ChurchMembers cm
                    WHERE cm.MemberID = m.MemberID AND cm.ChurchID = 1
                )
            ");

            migrationBuilder.Sql(@"
                UPDATE Servants SET ChurchID = 1
            ");

            migrationBuilder.Sql(@"
                UPDATE MemberFunds SET ChurchID = 1
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
