using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE Classes AS c
                                    JOIN Meetings AS m ON c.MeetingID = m.MeetingID
                                    SET 
                                        c.AgeStartDate = m.AgeStartDate,
                                        c.AgeEndDate   = m.AgeEndDate,
                                        c.Gender       = m.Gender,
                                        c.ChurchID       = m.ChurchID,
                                        c.ClassStartDate = m.MeetingStartDate,
                                        c.ClassEndDate = m.MeetingEndDate,
                                        c.ClassDay = m.MeetingDay,
                                        c.ClassStartTime = m.MeetingStartTime,
                                        c.ClassEndTime = m.MeetingEndTime,
                                        c.ClassFrequency = m.MeetingFrequency;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
