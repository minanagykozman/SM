using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Remove old view
            migrationBuilder.Sql("DROP VIEW IF EXISTS MemberEventView;");

            //Creat view with "Team" column
            migrationBuilder.Sql(@"CREATE VIEW MemberEventView AS
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
    m.Sequence AS Sequence,
    (CASE WHEN (er.MemberID IS NOT NULL) THEN 1 ELSE 0 END) AS Registered,
    e.EventID AS EventID,
    er.IsException AS IsException,
    er.Paid AS Paid,
    er.ServantID AS ServantID,
    er.TimeStamp AS TimeStamp,
    er.Notes AS RegistrationNotes,
    m.CardStatus AS CardStatus,
    er.Attended AS Attended,
    er.AttendanceServantID AS AttendanceServantID,
    er.AttendanceTimeStamp AS AttendanceTimeStamp,
    er.Team AS Team,
    er.Bus AS Bus,
    er.Room AS Room
FROM
    (
        (
            (
                (Members m JOIN ClassMembers cm ON((m.MemberID = cm.MemberID)))
                JOIN ClassEvents ce ON((ce.ClassID = cm.ClassID))
            )
            JOIN Events e ON((e.EventID = ce.EventID))
        )
        LEFT JOIN EventRegistrations er ON(
            (
                (er.MemberID = m.MemberID)
                AND (er.EventID = ce.EventID)
            )
        )
    )
WHERE
    (m.IsActive = 1)
UNION
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
    m.Sequence AS Sequence,
    (CASE WHEN (er.MemberID IS NOT NULL) THEN 1 ELSE 0 END) AS Registered,
    e.EventID AS EventID,
    er.IsException AS IsException,
    er.Paid AS Paid,
    er.ServantID AS ServantID,
    er.TimeStamp AS TimeStamp,
    er.Notes AS RegistrationNotes,
    m.CardStatus AS CardStatus,
    er.Attended AS Attended,
    er.AttendanceServantID AS AttendanceServantID,
    er.AttendanceTimeStamp AS AttendanceTimeStamp,
    er.Team AS Team,
    er.Bus AS Bus,
    er.Room AS Room
FROM
    (
        (Members m LEFT JOIN EventRegistrations er ON((er.MemberID = m.MemberID)))
        JOIN Events e ON((e.EventID = er.EventID))
    )
WHERE
    (m.IsActive = 1);
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
