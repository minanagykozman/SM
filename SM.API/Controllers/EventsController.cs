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
                return HandleError(ex);
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
                return HandleError(ex);
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
                return HandleError(ex);
            }
        }

        [HttpPost("Register")]
        public ActionResult<RegistrationStatus> Register(string memberCode, int eventID, bool isException, string? notes)
        {
            try
            {
                using (SM.BAL.EventHandler eventHandler = new SM.BAL.EventHandler())
                {
                    //ValidateServant();
                    var status = eventHandler.Register(memberCode, eventID, User.Identity.Name, isException, notes);
                    return Ok(status);
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
        }
    }
}
