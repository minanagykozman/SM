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
                    CardStatus cardStatus = (CardStatus)Enum.Parse(typeof(CardStatus), model.CardStatus);

                    eventHandler.UpdateCardStatus(model.MemberCode, cardStatus);
                    return Ok("Member updated");
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get member event registrations
        [HttpGet("GetMemberEventRegistrations")]
        public ActionResult<IEnumerable<EventRegistration>> GetMemberEventRegistrations(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    var registrations = memberHandler.GetMemberEventRegistrations(memberID);
                    return Ok(registrations);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get member class memberships
        [HttpGet("GetMemberClasses")]
        public ActionResult<IEnumerable<MemberClassOverview>> GetMemberClasses(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    var memberships = memberHandler.GetMemberClasses(memberID);
                    return Ok(memberships);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get member attendance history
        [HttpGet("GetAttendanceHistory")]
        public ActionResult<IEnumerable<ClassAttendance>> GetAttendanceHistory(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    var attendances = memberHandler.GetAttendanceHistory(memberID);
                    return Ok(attendances);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get member aid participations
        [HttpGet("GetMemberAids")]
        public ActionResult<IEnumerable<MemberAid>> GetMemberAids(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    var aids = memberHandler.GetMemberAids(memberID);
                    return Ok(aids);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get member fund transactions
        [HttpGet("GetMemberFunds")]
        public ActionResult<IEnumerable<MemberFund>> GetMemberFunds(int memberID)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    var funds = memberHandler.GetMemberFunds(memberID);
                    return Ok(funds);
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
            public string CardStatus { get; set; }

        }

    }
}
