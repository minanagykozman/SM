using Microsoft.AspNetCore.Mvc;
using SM.API.Controllers;
using System.Security.Claims;

namespace SM.API.Services
{
    public class SMControllerBase : ControllerBase
    {
        internal readonly ILogger<SMControllerBase> _logger;

        public SMControllerBase(ILogger<SMControllerBase> logger)
        {
            _logger = logger;
        }
        [NonAction]
        internal ActionResult HandleError(Exception ex)
        {
            _logger.LogError(ex, User?.Identity?.Name);
            if (ex.Source == "Show message")
            {
                var errorResponse = new { message = ex.Message };
                return StatusCode(500, errorResponse);
            }
            else
            {
                var errorResponse = new { message = "An error occured, contact system admin" };
                return StatusCode(500, errorResponse);
            }
        }
        [NonAction]
        internal void ValidateServant()
        {
            if (User == null)
                throw new Exception("User not logged in, unable to retrieve Servant details");
            if (User.Identity == null)
                throw new Exception("User not logged in, unable to retrieve Servant details");
            if (string.IsNullOrEmpty(User.Identity.Name))
                throw new Exception("User not logged in, unable to retrieve Servant details");
        }
    }
}
