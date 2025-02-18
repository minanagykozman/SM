using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "EventRegistrations",
                type: "varchar(25)",
                maxLength: 25,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
            //Remove old view
            migrationBuilder.Sql("DROP VIEW IF EXISTS MemberEventView;");

            //Creat view with "Team" column
            migrationBuilder.Sql(@"
            CREATE VIEW MemberEventView AS
            SELECT m.MemberID,m.Code, m.UNFirstName,m.UNLastName,m.UNFileNumber,m.UNPersonalNumber,m.Birthdate,
            m.Baptised,m.Gender,m.IsMainMember,m.Nickname,m.Mobile,m.Notes,m.ImageReference,
            (Case WHEN er.MemberID is not null THEN 1 else 0 END)as Registered,
            e.EventID,er.IsException,er.ServantID,er.TimeStamp, er.Notes as RegistrationNotes,
            er.Attended,er.AttendanceServantID,er.AttendanceTimeStamp,er.Team
            from Members m join ClassMembers cm on m.MemberID = cm.MemberID 
            join ClassEvents ce on ce.ClassID = cm.ClassID
            join Events e on e.EventID = ce.EventID
            left join EventRegistrations er on er.MemberID = m.MemberID and er.EventID = ce.EventID
            WHERE m.IsActive =1 ;
        ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Team",
                table: "EventRegistrations");
        }
    }
}
