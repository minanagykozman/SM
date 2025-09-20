using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SM.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventsView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP VIEW IF EXISTS membereventview;
CREATE ALGORITHM=UNDEFINED SQL SECURITY DEFINER VIEW `membereventview` AS select `m`.`MemberID` AS `MemberID`,
`m`.`Code` AS `Code`,`m`.`UNFirstName` AS `UNFirstName`,`m`.`UNLastName` AS `UNLastName`,
`m`.`UNFileNumber` AS `UNFileNumber`,`m`.`UNPersonalNumber` AS `UNPersonalNumber`,`m`.`Birthdate` AS `Birthdate`,
`m`.`Baptised` AS `Baptised`,`m`.`Gender` AS `Gender`,`m`.`IsMainMember` AS `IsMainMember`,
`m`.`Nickname` AS `Nickname`,`m`.`Mobile` AS `Mobile`,`m`.`Notes` AS `Notes`,`m`.`ImageReference` AS `ImageReference`,
`m`.`Sequence` AS `Sequence`,(case when (`er`.`MemberID` is not null) then 1 else 0 end) AS `Registered`,
`e`.`EventID` AS `EventID`,`er`.`IsException` AS `IsException` , `er`.`Paid` AS `Paid`,`er`.`ServantID` AS `ServantID`,
`er`.`TimeStamp` AS `TimeStamp`,`er`.`Notes` AS `RegistrationNotes`,`m`.`CardStatus` AS `CardStatus`,
`er`.`Attended` AS `Attended`,`er`.`AttendanceServantID` AS `AttendanceServantID`,
`er`.`AttendanceTimeStamp` AS `AttendanceTimeStamp`,`er`.`Team` AS `Team`,`er`.`Bus` AS `Bus` 
from ((((`members` `m` join `classmembers` `cm` on((`m`.`MemberID` = `cm`.`MemberID`))) 
join `classevents` `ce` on((`ce`.`ClassID` = `cm`.`ClassID`))) 
join `events` `e` on((`e`.`EventID` = `ce`.`EventID`))) 
left join `eventregistrations` `er` on(((`er`.`MemberID` = `m`.`MemberID`) and (`er`.`EventID` = `ce`.`EventID`))))
 where (`m`.`IsActive` = 1) 
 union select `m`.`MemberID` AS `MemberID`,`m`.`Code` AS `Code`,`m`.`UNFirstName` AS `UNFirstName`,
 `m`.`UNLastName` AS `UNLastName`,`m`.`UNFileNumber` AS `UNFileNumber`,`m`.`UNPersonalNumber` AS `UNPersonalNumber`,
 `m`.`Birthdate` AS `Birthdate`,`m`.`Baptised` AS `Baptised`,`m`.`Gender` AS `Gender`,
 `m`.`IsMainMember` AS `IsMainMember`,`m`.`Nickname` AS `Nickname`,`m`.`Mobile` AS `Mobile`,`m`.`Notes` AS `Notes`,
 `m`.`ImageReference` AS `ImageReference`,`m`.`Sequence` AS `Sequence`,
 (case when (`er`.`MemberID` is not null) then 1 else 0 end) AS `Registered`,`e`.`EventID` AS `EventID`,
 `er`.`IsException` AS `IsException`, `er`.`Paid` AS `Paid`,`er`.`ServantID` AS `ServantID`,
 `er`.`TimeStamp` AS `TimeStamp`,`er`.`Notes` AS `RegistrationNotes`,`m`.`CardStatus` AS `CardStatus`,`er`.`Attended` AS `Attended`,`er`.`AttendanceServantID` AS `AttendanceServantID`,`er`.`AttendanceTimeStamp` AS `AttendanceTimeStamp`,`er`.`Team` AS `Team`,`er`.`Bus` AS `Bus` from ((`members` `m` left join `eventregistrations` `er` on((`er`.`MemberID` = `m`.`MemberID`))) join `events` `e` on((`e`.`EventID` = `er`.`EventID`))) where (`m`.`IsActive` = 1);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
