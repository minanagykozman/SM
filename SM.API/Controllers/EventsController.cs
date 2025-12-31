using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EventsController(ILogger<EventsController> logger) : SMControllerBase(logger)
    {
        [Authorize(Policy = "Events.View")]

        [HttpGet("GetEvents")]
        public ActionResult<List<Event>> GetEvents()
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    List<Event> events = eventHandler.GetEvents(User.Identity.Name);
                    return Ok(events);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.View")]

        [HttpGet("GetEvent")]
        public ActionResult<Event> GetEvent(int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    Event? ev = eventHandler.GetEvent(eventID);
                    return Ok(ev);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpGet("CheckRegistrationStatus")]
        public ActionResult<RegistrationStatusResponse> CheckRegistrationStatus(string memberCode, int eventID)
        {
            try
            {
                Member member;
                List<MemberClassOverview> memberClasses=new List<MemberClassOverview>();
                RegistrationStatus status;
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    status = eventHandler.CheckRegistationStatus(memberCode, eventID, out member);
                }
                //using (SM.BAL.MemberHandler meetingHandler = new SM.BAL.MemberHandler())
                //{
                //    memberClasses = meetingHandler.GetMemberClassOverviews(member.MemberID);
                //}
                RegistrationStatusResponse response = new RegistrationStatusResponse() { Member = member, Status = status, MemberClasses = memberClasses };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Attendance")]
        [HttpGet("CheckAttendanceStatus")]
        public ActionResult<EventAttendanceStatusResponse> CheckAttendanceStatus(string memberCode, int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    MemberEventView member;
                    RegistrationStatus status = eventHandler.CheckEventAttendance(eventID, memberCode, out member);
                    EventAttendanceStatusResponse response = new EventAttendanceStatusResponse() { Member = member, Status = status };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.View")]
        [HttpGet("GetEventRegisteredMembers")]
        public ActionResult<List<MemberEventView>> GetEventRegisteredMembers(int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var members = eventHandler.GetEventMembers(eventID, true, null);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.View")]
        [HttpGet("GetEventAttendedMembers")]
        public ActionResult<List<Member>> GetEventAttendedMembers(int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var members = eventHandler.GetEventAttendedMembers(eventID);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.View")]
        [HttpGet("GetEventMembers")]
        public ActionResult<List<MemberEventView>> GetEventMembers(int eventID, bool? registered, bool? attended)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var members = eventHandler.GetEventMembers(eventID, registered, attended);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.View")]
        [HttpGet("DownloadEventMembers")]
        public IActionResult DownloadEventMembers(int eventID)
        {

            try
            {
                if (eventID <= 0)
                {
                    return BadRequest("Invalid event ID");
                }
                List<MemberEventView> members = new List<MemberEventView>();
                using (SM.BAL.EventHandler handler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    members = handler.GetEventMembers(eventID, true, null);
                }
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Event Members");

                // Add header
                worksheet.Cell(1, 1).Value = "Code";
                worksheet.Cell(1, 2).Value = "Full Name";
                worksheet.Cell(1, 3).Value = "Nickname";
                worksheet.Cell(1, 4).Value = "UN File Number";
                worksheet.Cell(1, 5).Value = "UN Personal Number";
                worksheet.Cell(1, 6).Value = "Mobile";
                worksheet.Cell(1, 7).Value = "Baptised";
                worksheet.Cell(1, 8).Value = "Birthdate";
                worksheet.Cell(1, 9).Value = "Age";
                worksheet.Cell(1, 10).Value = "Gender";
                worksheet.Cell(1, 11).Value = "Card Status";
                worksheet.Cell(1, 12).Value = "Status";
                worksheet.Cell(1, 13).Value = "Team";
                worksheet.Cell(1, 14).Value = "Bus";
                worksheet.Cell(1, 15).Value = "Room";
                worksheet.Cell(1, 16).Value = "Paid";
                worksheet.Cell(1, 17).Value = "Notes";

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
                    worksheet.Cell(row, 8).Value = member.Birthdate.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 9).Value = member.Age;
                    worksheet.Cell(row, 10).Value = member.Gender.ToString();
                    worksheet.Cell(row, 11).Value = member.CardStatus ?? "";
                    worksheet.Cell(row, 12).Value = member.Attended == null ? "Registered" : (member.Attended.Value ? "Present" : "Absent");
                    worksheet.Cell(row, 13).Value = member.Team ?? "";
                    worksheet.Cell(row, 14).Value = member.Bus ?? "";
                    worksheet.Cell(row, 15).Value = member.Room ?? "";
                    worksheet.Cell(row, 16).Value = member.Paid.HasValue ? member.Paid : "";
                    worksheet.Cell(row, 17).Value = member.Notes ?? "";

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                var fileName = $"EventMembers_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpPost("Register")]
        public ActionResult<RegistrationStatus> Register(string memberCode, float paid, int eventID, bool isException, string? notes)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    var status = eventHandler.Register(memberCode, eventID, paid, User.Identity.Name, isException, notes);
                    return Ok(status);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpPost("RemoveMember")]
        public ActionResult<RegistrationStatus> RemoveMember(int memberID, int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    eventHandler.RemoveMember(eventID, memberID, User.Identity.Name);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpPost("RemoveMemberAttendance")]
        public ActionResult<RegistrationStatus> RemoveMemberAttendance(int memberID, int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    eventHandler.RemoveMemberAttendance(eventID, memberID, User.Identity.Name);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpPost("UpdatePayment")]
        public ActionResult<RegistrationStatus> UpdatePayment(int memberID, int eventID, float paid)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    eventHandler.UpdatePaymentAmount(eventID, memberID, paid, User.Identity.Name);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpGet("UpdateAttendance")]
        public ActionResult<RegistrationStatus> UpdateAttendance(int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    eventHandler.UpdateAttendance(eventID);
                    return Ok();
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Manage")]
        [HttpPost("CreateEvent")]
        public ActionResult<int> CreateEvent([FromBody] EventParams eventParams)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var eventID = eventHandler.CreateEvent(eventParams.Event.EventName, eventParams.Event.EventStartDate, eventParams.Event.EventEndDate, eventParams.Classes);
                    return Ok(eventID);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Manage")]
        [HttpPost("DeleteEvent")]
        public ActionResult<int> DeleteEvent([FromBody] int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var id = eventHandler.DeleteEvent(eventID);
                    return Ok(id);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Manage")]
        [HttpPost("UpdateEvent")]
        public ActionResult<int> UpdateEvent([FromBody] EventParams eventParams)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var eventID = eventHandler.UpdateEvent(eventParams.Event.EventID, eventParams.Event.EventName, eventParams.Event.EventStartDate, eventParams.Event.EventEndDate, eventParams.Event.IsActive, eventParams.Classes);
                    return Ok(eventID);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Attendance")]
        [HttpPost("TakeAttendance")]
        public ActionResult<RegistrationStatus> TakeAttendance(string memberCode, int eventID,string busName)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    var status = eventHandler.TakeEventAttendance(eventID, memberCode, busName, User.Identity.Name);
                    return Ok(status);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Events.Register")]
        [HttpPost("DistributeMembers")]
        public ActionResult<string> DistributeMembers([FromBody] DistributionParams param)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    eventHandler.DistributeMembers(param.EventID, param.Teams, param.Busses);
                    return Ok("status");
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }


        public class RegistrationStatusResponse
        {
            public Member Member { get; set; }
            public RegistrationStatus Status { get; set; }
            public List<MemberClassOverview> MemberClasses { get; set; }
        }
        public class EventAttendanceStatusResponse
        {
            public MemberEventView Member { get; set; }
            public RegistrationStatus Status { get; set; }
        }
        public class DistributionParams
        {
            public int EventID { get; set; }
            public List<string> Teams { get; set; }
            public List<string> Busses { get; set; }
        }
        public class EventParams
        {
            public Event Event { get; set; }
            public List<int> Classes { get; set; }
        }
    }
}
