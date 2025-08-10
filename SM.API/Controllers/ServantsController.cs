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
        public ServantsController(ILogger<SMControllerBase> logger, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
       : base(logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [Authorize(Policy = "Servants.Manage")]
        [HttpGet("GetSystemRoles")]
        public ActionResult<List<IdentityRole>> GetSystemRoles()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
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
        [HttpGet("GetServants")]
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
                    var servant = handler.CreateServant(model.Name, model.Mobile, model.Mobile2, user.Id, model.Classes);
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
                    var servant = handler.UpdateServant(model.ServantID, model.Name, model.Mobile, model.Mobile2, model.Classes,model.Roles);
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
