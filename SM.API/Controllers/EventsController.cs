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
        [Authorize(Policy = "Events.Rgister")]
        [HttpGet("CheckRegistrationStatus")]
        public ActionResult<RegistrationStatusResponse> CheckRegistrationStatus(string memberCode, int eventID)
        {
            try
            {
                Member member;
                List<MemberClassOverview> memberClasses;
                RegistrationStatus status;
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    status = eventHandler.CheckRegistationStatus(memberCode, eventID, out member);
                }
                using (SM.BAL.MemberHandler meetingHandler = new SM.BAL.MemberHandler())
                {
                    memberClasses = meetingHandler.GetMemberClassOverviews(member.MemberID);
                }
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
        public ActionResult<List<Member>> GetEventRegisteredMembers(int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var members = eventHandler.GetEventRegisteredMembers(eventID);
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
        [Authorize(Policy = "Events.Register")]
        [HttpPost("Register")]
        public ActionResult<RegistrationStatus> Register(string memberCode, int eventID, bool isException, string? notes)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    var status = eventHandler.Register(memberCode, eventID, User.Identity.Name, isException, notes);
                    return Ok(status);
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
        public ActionResult<RegistrationStatus> TakeAttendance(string memberCode, int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    ValidateServant();
                    var status = eventHandler.TakeEventAttendance(eventID, memberCode, User.Identity.Name);
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
