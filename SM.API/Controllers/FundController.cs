using Microsoft.AspNetCore.Mvc;
using SM.BAL;
using SM.API.Services;
using SM.DAL.DataModel;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class FundController(ILogger<FundController> logger) : SMControllerBase(logger)
    {
        // GET: api/Fund
        [HttpGet]
        public ActionResult<List<MemberFund>> GetAllFunds(int? assigneeId = null, string? status = null)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var funds = fundHandler.GetAllFunds(assigneeId, status, User.Identity.Name);
                return Ok(funds);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // GET: api/Fund/my-assigned
        [HttpGet("my-assigned")]
        public IActionResult GetMyAssignedFunds([FromQuery] string? status)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var funds = fundHandler.GetMyAssignedFunds(User.Identity.Name, status);
                return Ok(funds);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // GET: api/Fund/by-status
        [HttpGet("by-status")]
        public IActionResult GetFundsByStatus([FromQuery] int? assigneeId)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var fundsByStatus = fundHandler.GetFundsByStatus(assigneeId, User.Identity.Name);
                return Ok(fundsByStatus);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // GET: api/Fund/{id}
        [HttpGet("{id}")]
        public IActionResult GetFund(int id)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var fund = fundHandler.GetFund(id, User.Identity.Name);
                if (fund == null)
                {
                    return NotFound("Fund not found or access denied");
                }
                return Ok(fund);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // POST: api/Fund
        [HttpPost]
        public IActionResult CreateFund([FromBody] CreateFundRequest request)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var fundId = fundHandler.CreateFund(request, User.Identity.Name);
                return Ok(new { FundID = fundId, Message = "Fund created successfully" });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // PUT: api/Fund/{id}/status
        [HttpPut("{id}/status")]
        public IActionResult UpdateFundStatus(int id, [FromBody] UpdateFundStatusRequest request)
        {
            try
            {
                request.FundID = id; // Ensure the ID matches the route
                FundHandler fundHandler = new SM.BAL.FundHandler();
                fundHandler.UpdateFundStatus(request, User.Identity.Name);
                return Ok(new { Message = "Fund status updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // POST: api/Fund/{id}/notes
        [HttpPost("{id}/notes")]
        public IActionResult AppendNotes(int id, [FromBody] AppendNotesRequest request)
        {
            try
            {
                request.FundID = id; // Ensure the ID matches the route
                FundHandler fundHandler = new SM.BAL.FundHandler();
                fundHandler.AppendNotes(request, User.Identity.Name);
                return Ok(new { Message = "Notes appended successfully" });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // PUT: api/Fund/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateFund(int id, [FromBody] UpdateFundRequest request)
        {
            try
            {
                request.FundID = id; // Ensure the ID matches the route
                FundHandler fundHandler = new SM.BAL.FundHandler();
                fundHandler.UpdateFund(request, User.Identity.Name);
                return Ok(new { Message = "Fund updated successfully" });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // DELETE: api/Fund/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteFund(int id)
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                fundHandler.DeleteFund(id, User.Identity.Name);
                return Ok(new { Message = "Fund deleted successfully" });
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // GET: api/Fund/statuses
        [HttpGet("statuses")]
        public IActionResult GetFundStatuses()
        {
            try
            {
                var statuses = Enum.GetValues<FundStatus>()
                    .Select(status => new { 
                        Value = status.ToString(), 
                        Label = status.ToString() 
                    })
                    .ToArray();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // GET: api/Fund/assignable-servants
        [HttpGet("assignable-servants")]
        public IActionResult GetAssignableServants()
        {
            try
            {
                FundHandler fundHandler = new SM.BAL.FundHandler();
                var servants = fundHandler.GetAssignableServants(User.Identity.Name);
                return Ok(servants);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
} 