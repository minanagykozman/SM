using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SM.BAL;
using SM.API.Services;
using SM.DAL.DataModel;
using System.Collections.Generic;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VisitationController(ILogger<FundController> logger) : SMControllerBase(logger)
    {
        [Authorize(Policy = "Visitation.View")]
        [HttpGet("get-visitations")]
        public IActionResult GetVisitations()
        {
            try
            {
                ValidateServant();
                using (VisitationHandler handler = new VisitationHandler())
                {
                    List<Visitation> visitations = new List<Visitation>();
                    if (User.IsInRole("SupperAdmin"))
                        visitations = handler.GetVisitations(null);
                    else
                        visitations = handler.GetVisitations(User.Identity.Name);
                    return Ok(visitations);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Visitation.View")]
        [HttpGet("get-family-attendance")]
        public IActionResult GetMemberFamilyAttendance(int memberID)
        {
            try
            {
                ValidateServant();
                using (VisitationHandler handler = new VisitationHandler())
                {
                    var visitations = handler.GetMemberFamilyAttendance(memberID);
                    return Ok(visitations);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Visitation.Manage")]
        [HttpPost("create-visitations")]
        public IActionResult CreateVisitations([FromBody] VisitationDto visitation)
        {
            try
            {
                ValidateServant();
                using (VisitationHandler handler = new VisitationHandler())
                {
                    var result = handler.AddVisitation(visitation.MemberIDs, User.Identity.Name, visitation.VisitationNotes, visitation.VisitationType, visitation.AssignedServant, visitation.ClassID);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Visitation.View")]
        [HttpPut("update-visitation")]
        public IActionResult UpdateVisitations([FromBody] UpdateVisitationDto visitation)
        {
            try
            {
                ValidateServant();
                using (VisitationHandler handler = new VisitationHandler())
                {
                    var result = handler.UpdateVisitationFeedback(visitation.VisitationID, visitation.MemberIDs, visitation.Feedback, visitation.Status, visitation.VisitationType, visitation.VisitationDate);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Visitation.Manage")]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteVisitation(int id)
        {
            try
            {
                using (VisitationHandler handler = new VisitationHandler())
                {
                    var result = handler.DeleteVisitation(id);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        
        public class VisitationDto
        {
            public List<int> MemberIDs { get; set; }
            public int AssignedServant { get; set; }
            public int? ClassID { get; set; }
            public string VisitationType { get; set; }
            public string VisitationNotes { get; set; }
        }
        public class UpdateVisitationDto
        {
            public int VisitationID { get; set; }
            public List<int> MemberIDs { get; set; }
            public string Feedback { get; set; }
            public string Status { get; set; }
            public string VisitationType { get; set; }
            public DateTime? VisitationDate { get; set; }
        }

    }
}