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

        [HttpGet("GetClasses/{servantID}")]
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
        [HttpGet("GetClasseOccurances/{classID}")]
        public ActionResult<List<ClassOccurrence>> GetClasseOccurances(int classID)
        {
            try
            {
                if (classID <= 0)
                {
                    return BadRequest("Invalid servantID");
                }
                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    List<ClassOccurrence> classes = classHandler.GetClassOccurances(classID);
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
        public ActionResult<MemberAttendanceResult> CheckAteendance(int classOccuranceID, string memberCode)
        {
            try
            {

                using (SM.BAL.MeetingHandler classHandler = new SM.BAL.MeetingHandler())
                {
                    Member member;
                    AttendanceStatus status = classHandler.CheckAteendance(classOccuranceID, memberCode, out member);

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
        [HttpPost("CreateClassOccurances")]
        public ActionResult<string> CreateClassOccurances(int classID)
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
        [HttpPost("CreateClassOccurancesTimed")]
        public ActionResult<List<ClassOccurrence>> CreateClassOccurancesTimed(int classID, DateTime startDate, DateTime endDate)
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
        public ActionResult<AttendanceStatus> TakeAttendance(int classOccuranceID, string memberCode, int servantID, bool forceRegister)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.TakeClassAteendance(classOccuranceID, memberCode, servantID, forceRegister);
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
        public ActionResult<string> TakeAttendanceBulk(int classOccuranceID, List<int> memberIDs, int servantID)
        {
            try
            {
                using (SM.BAL.MeetingHandler meetingHandler = new SM.BAL.MeetingHandler())
                {
                    var cl = meetingHandler.TakeClassAteendance(classOccuranceID, memberIDs, servantID);
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
