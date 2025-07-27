using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SixLabors.Fonts;
using SM.API.Services;
using SM.BAL;
using SM.DAL.DataModel;
using System.Collections.Generic;

using System.IO.Compression;

using static SM.BAL.MemberHandler;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SM.API.Authorization;


namespace SM.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PermissionsController : SMControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public PermissionsController(ILogger<SMControllerBase> logger, IAuthorizationService authorizationService)
        : base(logger)
        {
            _authorizationService = authorizationService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserPermissions()
        {
            using (AuthorizationHandler authorizationHandler = new AuthorizationHandler()) 
            {
                var allPermissionNames = await authorizationHandler.GetAllPermissionNamesAsync();
                var permissionsDto = new UserPermissionsDto();
                var tasks = new List<Task>();

                foreach (var permissionName in allPermissionNames)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var isAuthorized = (await _authorizationService.AuthorizeAsync(User, permissionName)).Succeeded;
                        permissionsDto.Permissions[permissionName] = isAuthorized;
                    }));
                }

                await Task.WhenAll(tasks);

                return Ok(permissionsDto);
            }
        }
    }
}
