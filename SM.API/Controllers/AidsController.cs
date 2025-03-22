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
    public class AidsController(ILogger<EventsController> logger) : SMControllerBase(logger)
    {

        [HttpGet("GetAids")]
        public ActionResult<List<Aid>> GetAids()
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    ValidateServant();
                    List<Aid> aids = aidHandler.GetAids(User.Identity.Name);
                    return Ok(aids);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }


        [HttpGet("CheckMemberStatus")]
        public ActionResult<AidStatusResponse> CheckMemberStatus(string memberCode, int aidID)
        {
            try
            {
                using (SM.BAL.AidHandler eventHandler = new SM.BAL.AidHandler())
                {
                    Member member;
                    AidStatus status = eventHandler.CheckMemberStatus(aidID, memberCode, out member);
                    AidStatusResponse response = new AidStatusResponse() { Member = member, Status = status };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }


        [HttpGet("GetAidMembers")]
        public ActionResult<List<MemberAid>> GetAidMembers(int aidID)
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    var members = aidHandler.GetAidMembers(aidID);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }


        [HttpPost("Register")]
        public ActionResult<AidStatus> Register(string memberCode, int aidID, string? notes)
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    ValidateServant();
                    var status = aidHandler.TakeAid(aidID, memberCode, notes, User.Identity.Name);
                    return Ok(status);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        


        public class AidStatusResponse
        {
            public Member Member { get; set; }
            public AidStatus Status { get; set; }
        }
        
    }
}
