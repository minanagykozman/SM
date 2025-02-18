using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {

        private readonly ILogger<EventsController> _logger;

        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetEvents")]
        public ActionResult<List<Event>> GetEvents()
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    List<Event> events = eventHandler.GetEvents(User.Identity.Name);
                    return Ok(events);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("CheckRegistrationStatus")]
        public ActionResult<RegistrationStatusResponse> CheckRegistrationStatus(string memberCode, int eventID)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    Member member;
                    RegistrationStatus status = eventHandler.CheckRegistationStatus(memberCode, eventID, out member);
                    RegistrationStatusResponse response = new RegistrationStatusResponse() { Member = member, Status = status };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Register")]
        public ActionResult<RegistrationStatus> Register(string memberCode, int eventID, bool isException, string? notes)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    var status = eventHandler.Register(memberCode, eventID, User.Identity.Name, isException, notes);
                    return Ok(status);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        public class RegistrationStatusResponse
        {
            public Member Member { get; set; }
            public RegistrationStatus Status { get; set; }
        }
    }
}
