using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberClasssAttendanceView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW MemberClasssAttendanceView AS
            SELECT m.MemberID,m.Code, m.UNFirstName,m.UNLastName,m.UNFileNumber,m.UNPersonalNumber,m.Birthdate,
            m.Baptised,m.Gender,m.IsMainMember,m.Nickname,m.Mobile,m.Notes,m.ImageReference,
            co.ClassID,co.ClassOccurrenceID,co.ClassOccurrenceStartDate,co.ClassOccurrenceEndDate,c.ClassName,
            (Case WHEN ca.MemberID is not null THEN 1 else 0 END)as Present,
            ca.ServantID,ca.TimeStamp
            from Members m join ClassMembers cm on m.MemberID = cm.MemberID 
            join Classes c on cm.ClassID = c.ClassID
            join ClassOccurrences co on cm.ClassID = co.ClassID
            left join ClassAttendances ca on ca.ClassOccurrenceID = co.ClassOccurrenceID AND ca.MemberID = m.MemberID
            WHERE m.IsActive =1 AND c.IsActive =1;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS MemberClasssAttendanceView;");
        }
    }
}
