using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AdjustAttendanceView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS MemberClasssAttendanceView;");
            migrationBuilder.Sql(@"
            CREATE VIEW MemberClasssAttendanceView AS
                SELECT
                    m.MemberID AS MemberID,
                    m.Code AS Code,
                    m.UNFirstName AS UNFirstName,
                    m.UNLastName AS UNLastName,
                    m.UNFileNumber AS UNFileNumber,
                    m.UNPersonalNumber AS UNPersonalNumber,
                    m.Birthdate AS Birthdate,
                    m.Baptised AS Baptised,
                    m.Gender AS Gender,
                    m.IsMainMember AS IsMainMember,
                    m.Nickname AS Nickname,
                    m.Mobile AS Mobile,
                    m.Notes AS Notes,
                    m.ImageReference AS ImageReference,
                    co.ClassID AS ClassID,
                    co.ClassOccurrenceID AS ClassOccurrenceID,
                    co.ClassOccurrenceStartDate AS ClassOccurrenceStartDate,
                    co.ClassOccurrenceEndDate AS ClassOccurrenceEndDate,
                    c.ClassName AS ClassName,
                    CASE
                        WHEN ca.MemberID IS NOT NULL THEN 1
                        ELSE 0
                    END AS Present,
                    ca.ServantID AS ServantID,
                    ca.TimeStamp AS TimeStamp
                FROM Members m
                INNER JOIN ClassMembers cm
                    ON m.MemberID = cm.MemberID
                INNER JOIN Classes c
                    ON cm.ClassID = c.ClassID
                INNER JOIN ClassOccurrences co
                    ON cm.ClassID = co.ClassID
                LEFT JOIN ClassAttendances ca
                    ON ca.ClassOccurrenceID = co.ClassOccurrenceID
                    AND ca.MemberID = m.MemberID;
        ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS MemberClasssAttendanceView;");
        }
    }
}
