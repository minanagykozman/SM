using Amazon.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static SM.API.Controllers.EventsController;
using static SM.BAL.EventHandler;

namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServantsController : SMControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        public ServantsController(ILogger<SMControllerBase> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IAuthorizationService authorizationService)
       : base(logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpGet("GetSystemRoles")]
        public async Task<ActionResult> GetSystemRoles()
        {
            try
            {
                bool admin = (await _authorizationService.AuthorizeAsync(User, "Church.ManageAll")).Succeeded;
                var roles = _roleManager.Roles.OrderBy(r => r.Name).ToList();
                if (!admin)
                {
                    roles.Remove(roles.Where(r => r.Name == "SuperAdmin").FirstOrDefault());
                    roles.Remove(roles.Where(r => r.Name == "ChurchAdmin").FirstOrDefault());
                }

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpGet("CheckEmail")]
        public ActionResult<bool> CheckEmail(string email)
        {
            try
            {
                using (ServantHandler handler = new ServantHandler())
                {
                    var valid = handler.CheckEmail(email);
                    return Ok(valid);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [HttpGet("get-servants")]
        public ActionResult<List<Servant>> GetServants(bool? isActive)
        {
            try
            {
                using (ServantHandler handler = new ServantHandler())
                {
                    var servants = handler.GetServants(isActive);
                    return Ok(servants);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpGet("GetServant")]
        public ActionResult<Servant> GetServant(int servantID)
        {
            try
            {
                using (ServantHandler handler = new ServantHandler())
                {
                    var servant = handler.GetServant(servantID);
                    return Ok(servant);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] ServantRegisterModel model)
        {
            try
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, model.Roles);
                }
                using (ServantHandler handler = new ServantHandler())
                {
                    int churchID = 0;
                    if (model.ChurchID.HasValue)
                        churchID = model.ChurchID.Value;
                    else
                    {
                        var creatingServant = handler.GetServantByUsername(User.Identity.Name);
                        churchID = creatingServant.ChurchID;
                    }
                    var servant = handler.CreateServant(model.Name, model.Mobile, model.Mobile2, user.Id, churchID, model.Classes);
                    return Ok(servant);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ServantEditModel model)
        {
            try
            {
                using (ServantHandler handler = new ServantHandler())
                {
                    var servant = handler.UpdateServant(model.ServantID, model.Name, model.Mobile, model.Mobile2, model.Classes, model.Roles);
                    return Ok(servant);
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
        public class ServantRegisterModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            public List<string> Roles { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Mobile2 { get; set; }
            public int? ChurchID { get; set; }
            public List<int> Classes { get; set; }
        }

        public class ServantEditModel
        {
            [Required]
            public int ServantID { get; set; }
            [Required]
            public List<string> Roles { get; set; }
            public string Name { get; set; }
            public string Mobile { get; set; }
            public string Mobile2 { get; set; }
            public List<int> Classes { get; set; }
        }
    }
}
