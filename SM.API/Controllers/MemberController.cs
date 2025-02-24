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
    //[Authorize]
    public class MemberController(ILogger<MemberController> logger) : SMControllerBase(logger)
    {


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
                return HandleError(ex);
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
                return HandleError(ex);
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
                return HandleError(ex);
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
                return HandleError(ex);
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
                return HandleError(ex);
            }
        }
        [HttpGet("GetMemberByCode")]
        public ActionResult<Member> GetMemberByCode(string memberCode)
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
                return HandleError(ex);
            }
        }
        [HttpPost("UpdateMember")]
        public ActionResult<string> UpdateMember([FromBody] Member member)
        {
            try
            {
                using (SM.BAL.MemberHandler eventHandler = new SM.BAL.MemberHandler())
                {
                    ValidateServant();
                    eventHandler.UpdateMember(member, User.Identity.Name);
                    return Ok("Member updated");
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateMember")]
        public ActionResult<string> CreateMember([FromBody] Member member)
        {
            try
            {
                using (SM.BAL.MemberHandler eventHandler = new SM.BAL.MemberHandler())
                {
                    ValidateServant();
                    string code = eventHandler.CreateMember(member, User.Identity.Name);
                    return Ok(code);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
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
                return HandleError(ex);
            }
        }

        public class UpdateCardModel
        {
            public string MemberCode { get; set; }
            public CardStatus CardStatus { get; set; }

        }

    }
}
