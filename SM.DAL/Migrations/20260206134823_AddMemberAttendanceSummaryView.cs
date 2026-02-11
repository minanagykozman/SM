using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberAttendanceSummaryView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS MemberAttendanceSummaryView;
                                    CREATE VIEW MemberAttendanceSummaryView AS
                                    SELECT 
                                        m.MemberID,
                                        c.ClassID,
                                        m.code,
                                        m.UNFileNumber,
                                        m.UNFirstName,
                                        m.UNLastName,
                                        c.ClassName,
                                        mi.MeetingStartDate,
                                        COUNT(ca.MemberID) AS AttendedTimes,
                                        MAX(co.ClassOccurrenceStartDate) AS LastAttendanceDate,
                                        (SELECT COUNT(*) 
                                         FROM ClassOccurrences co2 
                                         WHERE co2.ClassID = c.ClassID 
                                         AND co2.ClassOccurrenceStartDate <= CURDATE()) AS TotalOccurrencesToDate
                                    FROM Members m 
                                    JOIN ClassAttendances ca ON m.MemberID = ca.MemberID 
                                    JOIN ClassOccurrences co ON ca.ClassOccurrenceID = co.ClassOccurrenceID
                                    JOIN Classes c ON co.ClassID = c.ClassID
                                    JOIN Meetings mi ON c.MeetingID = mi.MeetingID
                                    GROUP BY 
                                        m.MemberID, 
                                        m.code, 
                                        c.ClassName, 
                                        c.ClassID, 
                                        mi.MeetingStartDate;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS MemberAttendanceSummaryView;");
        }
    }
}
