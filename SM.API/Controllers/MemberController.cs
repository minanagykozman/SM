using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.DAL.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MemberController : SMControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        public MemberController(ILogger<SMControllerBase> logger, IAmazonS3 s3Client)
       : base(logger)
        {
            _s3Client = s3Client;
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
                return HandleError(ex);
            }
        }
        [HttpGet("GetAllCodes")]
        public ActionResult<List<object>> GetAllCodes()
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    List<object> members = memberHandler.GetAllCodes();
                    return Ok(members);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("ValidateUNNumber")]
        public ActionResult<bool> ValidateUNNumber(string unFileNumber, int? memberID)
        {
            try
            {
                using ( SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    bool result = memberHandler.ValidateUNNumber(unFileNumber, memberID);
                    var json= new JsonResult(result);
                    return json;
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
        [HttpPost("UpdateMemberWImage")]
        public async Task<IActionResult> UpdateMemberWImage([FromForm] MemberParam param)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    ValidateServant();
                    string url = string.Empty;
                    string key = string.Empty;
                    if (param.ImageFile != null)
                    {
                        var imageData = await AWSHelper.UploadMemberImage(param.ImageFile, _s3Client, memberHandler);
                        url = imageData.URL;
                        key = imageData.Key;
                        if (!string.IsNullOrEmpty(param.S3Key))
                            await AWSHelper.DeleteFileAsync(param.S3Key, _s3Client);
                    }
                    else
                    {
                        url = param.ImageURL;
                        key = param.S3Key;
                    }


                    memberHandler.UpdateMember(param.MemberID.Value, param.Code, param.UNFirstName, param.UNLastName, param.BaptismName, param.Nickname
                        , param.UNFileNumber, param.UNPersonalNumber, param.Mobile, param.Baptised, param.Birthdate, param.Gender, param.School, param.Work,
                        param.Notes, url, key, param.ImageFile?.FileName, param.CardStatus, param.Classes, User.Identity.Name);

                    return Ok("Member Updated!");
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
                    Member newMember = eventHandler.CreateMember(member, User.Identity.Name);

                    return Ok(newMember.Code);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateMemberWImage")]
        public async Task<IActionResult> CreateMemberWImage([FromForm] MemberParam param)
        {
            try
            {
                using (SM.BAL.MemberHandler memberHandler = new SM.BAL.MemberHandler())
                {
                    ValidateServant();
                    var imageData = await AWSHelper.UploadMemberImage(param.ImageFile, _s3Client, memberHandler);

                    Member newMember = memberHandler.CreateMember(param.UNFirstName, param.UNLastName, param.BaptismName, param.Nickname
                        , param.UNFileNumber, param.UNPersonalNumber, param.Mobile, param.Baptised, param.Birthdate, param.Gender, param.School, param.Work,
                        param.Notes, imageData?.URL, imageData?.Key, param.ImageFile?.FileName, param.Classes, User.Identity.Name);

                    return Ok(newMember.Code);
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
        public class MemberParam
        {
            public IFormFile? ImageFile { get; set; }
            public int? MemberID { get; set; }
            public string? Code { get; set; }
            public string? UNFirstName { get; set; }
            public string? UNLastName { get; set; }
            public string? BaptismName { get; set; }
            public string? Nickname { get; set; }
            public string UNFileNumber { get; set; }
            public string UNPersonalNumber { get; set; }
            public string? Mobile { get; set; }
            public bool Baptised { get; set; }
            public DateTime Birthdate { get; set; }
            public char Gender { get; set; }
            public string? School { get; set; }
            public string? Work { get; set; }
            public string? Notes { get; set; }
            public string? S3Key { get; set; }
            public string? ImageURL { get; set; }
            public string? CardStatus { get; set; }
            public List<int> Classes { get; set; }
        }
    }
}
