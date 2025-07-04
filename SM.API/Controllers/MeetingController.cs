using ClosedXML.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MeetingController(ILogger<MeetingController> logger) : SMControllerBase(logger)
    {

        [HttpGet("GetServantClasses")]
        public ActionResult<List<Class>> GetServantClasses()
        {
            try
            {
                ValidateServant();
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Class> classes = classHandler.GetServantClasses(User.Identity.Name);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetClassOccurences")]
        public ActionResult<List<ClassOccurrence>> GetClassOccurences(int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    List<ClassOccurrence> classes = meetingHandler.GetClassOccurences(classID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetClassMembers")]
        public ActionResult<List<ClassMemberExtended>> GetClassMembers(int classID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    List<ClassMemberExtended> classes = meetingHandler.GetClassMembers(classID, User.Identity.Name);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetAttendedMembers")]
        public ActionResult<List<Member>> GetAttendedMembers(int occurenceID)
        {
            try
            {
                if (occurenceID <= 0)
                {
                    return BadRequest("Invalid servantID");
                }
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Member> classes = classHandler.GetAttendedMembers(occurenceID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("CheckAttendance")]
        public ActionResult<MemberAttendanceResult> CheckAttendance(int classOccurenceID, string memberCode)
        {
            try
            {

                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    Member member;
                    AttendanceStatus status = classHandler.CheckAteendance(classOccurenceID, memberCode, out member);
                    MemberAttendanceResult response = new MemberAttendanceResult() { AttendanceStatus = status, Member = member };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClass")]
        public ActionResult<Class> CreateClass(string className, int meetingID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClass(className, meetingID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("AutoAssignClassMembers")]
        public ActionResult<string> AutoAssignClassMembers(int classID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.AutoAssignClassMembers(classID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClassOccurences")]
        public ActionResult<string> CreateClassOccurences(int classID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClassOccurences(classID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClassOccurencesTimed")]
        public ActionResult<List<ClassOccurrence>> CreateClassOccurencesTimed(int classID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClassOccurences(classID, startDate, endDate);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("TakeAttendance")]
        public ActionResult<AttendanceStatus> TakeAttendance(int classOccurenceID, string memberCode, bool forceRegister)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberCode, User.Identity.Name, forceRegister);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("TakeAttendanceBulk")]
        public ActionResult<string> TakeAttendanceBulk(int classOccurenceID, List<int> memberIDs, int servantID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberIDs, User.Identity.Name);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("AssignMemberServant")]
        public ActionResult<string> AssignMemberServant(int classID, int memberID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.AssignMemberToServant(memberID, classID, User.Identity.Name);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("UnAssignMemberServant")]
        public ActionResult<string> UnAssignMemberServant(int classID, int memberID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.UnAssignMemberServant(memberID, classID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpGet("DownloadClassMembers")]
        public IActionResult DownloadClassMembers(int classID)
        {

            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }
                List<ClassMemberExtended> members = new List<ClassMemberExtended>();
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    members = meetingHandler.GetClassMembers(classID, User.Identity.Name);
                }
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Class Members");

                // Add header
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Full Name";
                worksheet.Cell(1, 3).Value = "Nickname";
                worksheet.Cell(1, 4).Value = "UN File Number";
                worksheet.Cell(1, 5).Value = "UN Personal Number";
                worksheet.Cell(1, 6).Value = "Mobile";
                worksheet.Cell(1, 7).Value = "Baptised";
                worksheet.Cell(1, 8).Value = "Baptism Name";
                worksheet.Cell(1, 9).Value = "Birthdate";
                worksheet.Cell(1, 10).Value = "Age";
                worksheet.Cell(1, 11).Value = "Gender";
                worksheet.Cell(1, 12).Value = "School";
                worksheet.Cell(1, 13).Value = "Work";
                worksheet.Cell(1, 14).Value = "Is Main Member";
                worksheet.Cell(1, 15).Value = "Is Active";
                worksheet.Cell(1, 16).Value = "Card Status";
                worksheet.Cell(1, 17).Value = "Card Delivery Count";
                worksheet.Cell(1, 18).Value = "Notes";
                worksheet.Cell(1, 19).Value = "Last Present Date";
                worksheet.Cell(1, 20).Value = "Attendance";
                worksheet.Cell(1, 21).Value = "Servant";

                int row = 2;
                foreach (var member in members)
                {
                    worksheet.Cell(row, 1).Value = member.Code;
                    worksheet.Cell(row, 2).Value = member.FullName;
                    worksheet.Cell(row, 3).Value = member.Nickname ?? "";
                    worksheet.Cell(row, 4).Value = member.UNFileNumber;
                    worksheet.Cell(row, 5).Value = member.UNPersonalNumber;
                    worksheet.Cell(row, 6).Value = member.Mobile ?? "";
                    worksheet.Cell(row, 7).Value = member.Baptised ? "Yes" : "No";
                    worksheet.Cell(row, 8).Value = member.BaptismName ?? "";
                    worksheet.Cell(row, 9).Value = member.Birthdate.ToShortDateString();
                    worksheet.Cell(row, 10).Value = member.Age;
                    worksheet.Cell(row, 11).Value = member.Gender.ToString();
                    worksheet.Cell(row, 12).Value = member.School ?? "";
                    worksheet.Cell(row, 13).Value = member.Work ?? "";
                    worksheet.Cell(row, 14).Value = member.IsMainMember ? "Yes" : "No";
                    worksheet.Cell(row, 15).Value = member.IsActive ? "Yes" : "No";
                    worksheet.Cell(row, 16).Value = member.CardStatus ?? "";
                    worksheet.Cell(row, 17).Value = member.CardDeliveryCount;
                    worksheet.Cell(row, 18).Value = member.Notes ?? "";
                    worksheet.Cell(row, 19).Value = member.LastPresentDate?.ToShortDateString() ?? "";
                    worksheet.Cell(row, 20).Value = member.Attendance ?? "";
                    worksheet.Cell(row, 21).Value = member.Servant ?? "";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var fileName = $"ClassMembers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        public class MemberAttendanceResult
        {
            public AttendanceStatus AttendanceStatus { get; set; }
            public Member Member { get; set; }
        }
    }
}
