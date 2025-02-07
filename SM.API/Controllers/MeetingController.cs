using Microsoft.AspNetCore.Mvc;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeetingController : ControllerBase
    {

        private readonly ILogger<MeetingController> _logger;

        public MeetingController(ILogger<MeetingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetClasses")]
        public ActionResult<List<Class>> GetServantClasses(int servantID)
        {
            try
            {
                if (servantID <= 0)
                {
                    return BadRequest("Invalid servantID");
                }
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<Class> classes = classHandler.GetServantClasses(servantID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetClasseOccurences")]
        public ActionResult<List<ClassOccurrence>> GetClasseOccurences(int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid servantID");
                }
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    List<ClassOccurrence> classes = meetingHandler.GetClassOccurences(classID);
                    return Ok(classes);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("CheckAteendance")]
        public ActionResult<MemberAttendanceResult> CheckAteendance(int classOccurenceID, string memberCode)
        {
            try
            {

                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    Member member;
                    AttendanceStatus status = classHandler.CheckAteendance(classOccurenceID, memberCode, out member);

                    return Ok(new MemberAttendanceResult() { AttendanceStatus = status, Member = member });
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("TakeAttendance")]
        public ActionResult<AttendanceStatus> TakeAttendance(int classOccurenceID, string memberCode, int servantID, bool forceRegister)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberCode, servantID, forceRegister);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost("TakeAttendanceBulk")]
        public ActionResult<string> TakeAttendanceBulk(int classOccurenceID, List<int> memberIDs, int servantID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.TakeClassAteendance(classOccurenceID, memberIDs, servantID);
                    return Ok(cl);
                }

            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        public class MemberAttendanceResult
        {
            public AttendanceStatus AttendanceStatus { get; set; }
            public Member Member { get; set; }
        }
    }
}
