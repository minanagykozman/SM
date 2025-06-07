using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.API.Controllers.EventsController;
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
        [HttpGet("GetAid")]
        public ActionResult<Aid> GetAid(int aidID)
        {
            try
            {
                using (SM.BAL.AidHandler eventHandler = new SM.BAL.AidHandler())
                {
                    ValidateServant();
                    Aid? ev = eventHandler.GetAid(aidID);
                    return Ok(ev);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("DeleteAid")]
        public ActionResult<int> DeleteAid([FromBody] int aidID)
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    var id = aidHandler.DeleteAid(aidID);
                    return Ok(id);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("UpdateAid")]
        public ActionResult<int> UpdateAid([FromBody] AidParams aidParams)
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    var id = aidHandler.UpdateAid(aidParams.Aid,aidParams.Classes);
                    return Ok(id);
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
        [HttpPost("Create")]
        public ActionResult<int> Create([FromBody] AidParams aidParams)
        {
            try
            {
                using (SM.BAL.AidHandler aidHandler = new SM.BAL.AidHandler())
                {
                    int aidID = aidHandler.CreateAid(aidParams.Aid, aidParams.Classes);
                    return Ok(aidID);
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
        public class AidParams
        {
            public Aid Aid { get; set; }
            public List<int> Classes { get; set; }
        }
    }
}
