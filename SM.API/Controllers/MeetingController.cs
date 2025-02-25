using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MeetingController(ILogger<MeetingController> logger) : SMControllerBase(logger)
    {

        [HttpGet("GetClasses")]
        public ActionResult<List<Class>> GetServantClasses()
        {
            try
            {
                ValidateServant();
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Class> classes = classHandler.GetServantClasses(User.Identity.Name);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetClassOccurences")]
        public ActionResult<List<ClassOccurrence>> GetClassOccurences(int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    List<ClassOccurrence> classes = meetingHandler.GetClassOccurences(classID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetClassMembers")]
        public ActionResult<List<Member>> GetClassMembers(int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid class ID");
                }
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    List<Member> classes = meetingHandler.GetClassMembers(classID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("GetAttendedMembers")]
        public ActionResult<List<Member>> GetAttendedMembers(int occurenceID)
        {
            try
            {
                if (occurenceID <= 0)
                {
                    return BadRequest("Invalid servantID");
                }
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Member> classes = classHandler.GetAttendedMembers(occurenceID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("CheckAttendance")]
        public ActionResult<MemberAttendanceResult> CheckAttendance(int classOccurenceID, string memberCode)
        {
            try
            {

                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    Member member;
                    AttendanceStatus status = classHandler.CheckAteendance(classOccurenceID, memberCode, out member);
                    MemberAttendanceResult response = new MemberAttendanceResult() { AttendanceStatus = status, Member = member };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClass")]
        public ActionResult<Class> CreateClass(string className, int meetingID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClass(className, meetingID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClassOccurences")]
        public ActionResult<string> CreateClassOccurences(int classID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClassOccurences(classID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("CreateClassOccurencesTimed")]
        public ActionResult<List<ClassOccurrence>> CreateClassOccurencesTimed(int classID, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.CreateClassOccurences(classID, startDate, endDate);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("TakeAttendance")]
        public ActionResult<AttendanceStatus> TakeAttendance(int classOccurenceID, string memberCode, bool forceRegister)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberCode, User.Identity.Name, forceRegister);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpPost("TakeAttendanceBulk")]
        public ActionResult<string> TakeAttendanceBulk(int classOccurenceID, List<int> memberIDs, int servantID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    ValidateServant();
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberIDs, User.Identity.Name);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        public class MemberAttendanceResult
        {
            public AttendanceStatus AttendanceStatus { get; set; }
            public Member Member { get; set; }
        }
    }
}
