using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.DAL.DataModel;
using static SM.BAL.MeetingHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MeetingController(ILogger<MeetingController> logger) : SMControllerBase(logger)
    {
        [Authorize(Policy = "Class.View")]
        [HttpGet("GetServantClasses")]
        public ActionResult<List<Class>> GetServantClasses(bool? isActive)
        {
            try
            {
                ValidateServant();
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Class> classes = new List<Class>();
                    if (User.IsInRole("SuperAdmin"))
                        classes = classHandler.GetAllClasses(isActive, User.Identity.Name);
                    else
                        classes = classHandler.GetServantClasses(User.Identity.Name);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Attendance")]
        [HttpGet("get-class-occurance-membres")]
        public ActionResult<List<MemberClasssAttendanceView>> GetClassOccurenceMembers(int occurenceID)
        {
            try
            {
                if (occurenceID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    List<MemberClasssAttendanceView> classes = meetingHandler.GetClassOccurenceMembers(occurenceID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.View")]
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
        [Authorize(Policy = "Class.View")]
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
        [Authorize(Policy = "Class.View")]
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
        [Authorize(Policy = "Class.Attendance")]
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
        [Authorize(Policy = "Class.Attendance")]
        [HttpGet("get-meeting-data")]
        public ActionResult<MeetingDataDto> CheckAttendance(int classOccurenceID)
        {
            try
            {

                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    var data = classHandler.GetMeetingData(classOccurenceID);
                    return Ok(data);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Manage")]
        [HttpPost("CreateClass")]
        public ActionResult<Class> CreateClass([FromBody] ClassDTO classData)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClass(User.Identity.Name, classData.ClassName, classData.AgeStartDate, classData.AgeEndDate, classData.Gender, classData.ClassStartDate, classData.ClassEndDate, classData.ClassDay, classData.ClassStartTime, classData.ClassEndTime, classData.ClassFrequency, classData.Notes, classData.Year);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Manage")]
        [HttpPost("edit-class")]
        public ActionResult<Class> EditClass([FromBody] ClassDTO classData)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.EditClass(User.Identity.Name, classData.ClassID.Value, classData.ClassName, classData.AgeStartDate, classData.AgeEndDate, classData.Gender, classData.ClassStartDate, classData.ClassEndDate, classData.ClassDay, classData.ClassStartTime, classData.ClassEndTime, classData.ClassFrequency, classData.Notes, classData.Year, classData.IsActive.Value);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Manage")]
        [HttpPost("auto-assign-class-members")]
        public ActionResult<string> AutoAssignClassMembers([FromBody] int classID)
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

        [Authorize(Policy = "Class.Manage")]
        [HttpPost("auto-remove-class-members")]
        public ActionResult<string> AutoRemoveClassMembers([FromBody] int classID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.AutoRemoveClassMembers(classID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Manage")]
        [HttpPost("create-class-occurences")]
        public ActionResult<string> CreateClassOccurences([FromBody] int classID)
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
        [Authorize(Policy = "Class.Manage")]
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
        [Authorize(Policy = "Class.Attendance")]
        [HttpPost("TakeAttendance")]
        public ActionResult<int> TakeAttendance(int classOccurenceID, string memberCode, bool forceRegister)
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
        [Authorize(Policy = "Class.Attendance")]
        [HttpPost("remove-attendance")]
        public ActionResult<int> RemoveAttendance(int classOccurenceID, string memberCode)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.RemoveClassAteendance(classOccurenceID, memberCode, User.Identity.Name);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.Attendance")]
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
        [Authorize(Policy = "Class.ManageVisitation")]
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
        [Authorize(Policy = "Class.ManageVisitation")]
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
        [Authorize(Policy = "Class.View")]
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
                worksheet.Cell(1, 21).Value = "Attendance Count";
                worksheet.Cell(1, 22).Value = "Assigned Servant";

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
                    worksheet.Cell(row, 21).Value = member.AttendanceCounter;
                    worksheet.Cell(row, 22).Value = member.Servant ?? "";

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var fileName = $"ClassMembers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Class.View")]
        [HttpPost("download-class-attendance")]
        public async Task<IActionResult> DownloadClassAttendance([FromBody]int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }

                List<MemberClasssAttendanceView> attendanceData = new List<MemberClasssAttendanceView>();
                List<ClassMemberExtended> members = new List<ClassMemberExtended>();
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    attendanceData = meetingHandler.GetClassMembers(classID);
                    members = meetingHandler.GetClassMembers(classID, User.Identity.Name);
                }

                if (attendanceData == null || !attendanceData.Any())
                {
                    return NotFound("No attendance records found for this class.");
                }

                using var workbook = new XLWorkbook();

                var sheetMembers = workbook.Worksheets.Add("Class Members");

                // Add header
                sheetMembers.Cell(1, 1).Value = "Code";
                sheetMembers.Cell(1, 2).Value = "Full Name";
                sheetMembers.Cell(1, 3).Value = "Nickname";
                sheetMembers.Cell(1, 4).Value = "UN File Number";
                sheetMembers.Cell(1, 5).Value = "UN Personal Number";
                sheetMembers.Cell(1, 6).Value = "Mobile";
                sheetMembers.Cell(1, 7).Value = "Baptised";
                sheetMembers.Cell(1, 8).Value = "Baptism Name";
                sheetMembers.Cell(1, 9).Value = "Birthdate";
                sheetMembers.Cell(1, 10).Value = "Age";
                sheetMembers.Cell(1, 11).Value = "Gender";
                sheetMembers.Cell(1, 12).Value = "School";
                sheetMembers.Cell(1, 13).Value = "Work";
                sheetMembers.Cell(1, 14).Value = "Is Main Member";
                sheetMembers.Cell(1, 15).Value = "Is Active";
                sheetMembers.Cell(1, 16).Value = "Card Status";
                sheetMembers.Cell(1, 17).Value = "Card Delivery Count";
                sheetMembers.Cell(1, 18).Value = "Notes";
                sheetMembers.Cell(1, 19).Value = "Last Present Date";
                sheetMembers.Cell(1, 20).Value = "Attendance";
                sheetMembers.Cell(1, 21).Value = "Attendance Count";
                sheetMembers.Cell(1, 22).Value = "Assigned Servant";

                int row = 2;
                foreach (var member in members)
                {
                    sheetMembers.Cell(row, 1).Value = member.Code;
                    sheetMembers.Cell(row, 2).Value = member.FullName;
                    sheetMembers.Cell(row, 3).Value = member.Nickname ?? "";
                    sheetMembers.Cell(row, 4).Value = member.UNFileNumber;
                    sheetMembers.Cell(row, 5).Value = member.UNPersonalNumber;
                    sheetMembers.Cell(row, 6).Value = member.Mobile ?? "";
                    sheetMembers.Cell(row, 7).Value = member.Baptised ? "Yes" : "No";
                    sheetMembers.Cell(row, 8).Value = member.BaptismName ?? "";
                    sheetMembers.Cell(row, 9).Value = member.Birthdate.ToShortDateString();
                    sheetMembers.Cell(row, 10).Value = member.Age;
                    sheetMembers.Cell(row, 11).Value = member.Gender.ToString();
                    sheetMembers.Cell(row, 12).Value = member.School ?? "";
                    sheetMembers.Cell(row, 13).Value = member.Work ?? "";
                    sheetMembers.Cell(row, 14).Value = member.IsMainMember ? "Yes" : "No";
                    sheetMembers.Cell(row, 15).Value = member.IsActive ? "Yes" : "No";
                    sheetMembers.Cell(row, 16).Value = member.CardStatus ?? "";
                    sheetMembers.Cell(row, 17).Value = member.CardDeliveryCount;
                    sheetMembers.Cell(row, 18).Value = member.Notes ?? "";
                    sheetMembers.Cell(row, 19).Value = member.LastPresentDate?.ToShortDateString() ?? "";
                    sheetMembers.Cell(row, 20).Value = member.Attendance ?? "";
                    sheetMembers.Cell(row, 21).Value = member.AttendanceCounter;
                    sheetMembers.Cell(row, 22).Value = member.Servant ?? "";

                    row++;
                }

                sheetMembers.Columns().AdjustToContents();


                var attendanceSheet = workbook.Worksheets.Add("Members Attendance");

                // 1. Extract all unique class occurrence dates sorted chronologically for dynamic columns
                var distinctDates = attendanceData
                    .Select(x => x.ClassOccurrenceStartDate.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // 2. Set up Static Headers
                attendanceSheet.Cell(1, 1).Value = "Code";
                attendanceSheet.Cell(1, 2).Value = "Fullname";

                // 3. Set up Dynamic Date Columns (Formatted as "MMM-dd-yy")
                int dateColStart = 3;
                for (int i = 0; i < distinctDates.Count; i++)
                {
                    var colIndex = dateColStart + i;
                    var cell = attendanceSheet.Cell(1, colIndex);
                    cell.Value = distinctDates[i];
                    cell.Style.NumberFormat.Format = "MMM-dd-yy";
                    cell.Style.Alignment.TextRotation = 90;
                }

                // 4. Set up Total Column Header
                int totalColIndex = dateColStart + distinctDates.Count;
                attendanceSheet.Cell(1, totalColIndex).Value = "Total";

                // 5. Group data by each unique member using Code and FullName properties
                var memberGroups = attendanceData
                    .GroupBy(x => new { x.MemberID, x.Code, x.FullName })
                    .ToList();

                row = 2;
                foreach (var group in memberGroups)
                {
                    // Write Code and FullName
                    attendanceSheet.Cell(row, 1).Value = group.Key.Code ?? "";
                    attendanceSheet.Cell(row, 2).Value = group.Key.FullName ?? "";

                    // Create a lookup set of dates where this specific member was present
                    var presentDates = group
                        .Where(x => x.Present)
                        .Select(x => x.ClassOccurrenceStartDate.Date)
                        .ToHashSet();

                    // Fill in "+" signs under the respective date columns
                    for (int i = 0; i < distinctDates.Count; i++)
                    {
                        var colIndex = dateColStart + i;
                        var currentDate = distinctDates[i];

                        if (presentDates.Contains(currentDate))
                        {
                            var cell = attendanceSheet.Cell(row, colIndex);
                            cell.Value = "+";
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }
                    }

                    // Add the Total formula counting "+" across the row's date cells using COUNTIF
                    string startColLetter = attendanceSheet.Column(dateColStart).ColumnLetter();
                    string endColLetter = attendanceSheet.Column(dateColStart + distinctDates.Count - 1).ColumnLetter();

                    attendanceSheet.Cell(row, totalColIndex).FormulaA1 = $"COUNTIF({startColLetter}{row}:{endColLetter}{row}, \"+\")";

                    row++;
                }

                // Apply consistent styling to the header row
                var headerRange = attendanceSheet.Range(1, 1, 1, totalColIndex);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                attendanceSheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var fileName = $"ClassAttendance_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
        public class ClassDTO
        {
            public int? ClassID { get; set; }
            public string ClassName { get; set; }
            public DateOnly? AgeStartDate { get; set; }
            public DateOnly? AgeEndDate { get; set; }
            public char Gender { get; set; }
            public DateTime? ClassStartDate { get; set; }
            public DateTime? ClassEndDate { get; set; }
            public string ClassDay { get; set; } = string.Empty;
            public string ClassStartTime { get; set; } = string.Empty;
            public string ClassEndTime { get; set; } = string.Empty;
            public string ClassFrequency { get; set; } = string.Empty;
            public string? Notes { get; set; }
            public int Year { get; set; }
            public bool? IsActive { get; set; }
        }
    }
}
