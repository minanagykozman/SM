using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[Authorize]
    public class MemberController : ControllerBase
    {

        private readonly ILogger<EventsController> _logger;

        public MemberController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetFamily")]
        public ActionResult<List<Member>> GetFamily(string unFileNumber)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    List<Member> members = memberHandler.GetFamilyByUNFileNumber(unFileNumber);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("ValidateUNNumber")]
        public ActionResult<bool> ValidateUNNumber(string unFileNumber)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    bool result = memberHandler.ValidateUNNumber(unFileNumber);
                    return Ok(result);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("SearchMembers")]
        public ActionResult<List<Member>> SearchMembers(string? memberCode, string? firstName, string? lastName)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    List<Member> members = memberHandler.GetMembers(memberCode, firstName, lastName);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetMembersByCardStatus")]
        public ActionResult<List<Member>> GetMembersByCardStatus(CardStatus cardStatus)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    List<Member> members = memberHandler.GetMembersByCardStatus(cardStatus);
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetMember")]
        public ActionResult<Member> GetMember(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    Member member = memberHandler.GetMember(memberID);
                    return Ok(member);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetMemberByCode")]
        public ActionResult<Member> GetMemberByCode([FromBody] string memberCode)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    Member member = memberHandler.GetMemberByCodeOnly(memberCode);
                    return Ok(member);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("UpdateMember")]
        public ActionResult<string> UpdateMember(Member member)
        {
            try
            {
                using (SM.BAL.MemberHandler eventHandler = new SM.BAL.MemberHandler())
                {
                    eventHandler.UpdateMember(member);
                    return Ok("Member updated");
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("UpdateMemberCard")]
        public ActionResult<string> UpdateMemberCard([FromBody] UpdateCardModel model)
        {
            try
            {
                using (SM.BAL.MemberHandler eventHandler = new SM.BAL.MemberHandler())
                {
                    eventHandler.UpdateCardStatus(model.MemberCode, model.CardStatus);
                    return Ok("Member updated");
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public class  UpdateCardModel
        {
            public string MemberCode { get; set; }
            public CardStatus CardStatus { get; set; }
            
        }

    }
}
