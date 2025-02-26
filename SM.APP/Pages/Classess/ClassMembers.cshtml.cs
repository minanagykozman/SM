using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Classess
{
    public class ClassMembersModel(UserManager<IdentityUser> userManager, ILogger<ClassMembersModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty(SupportsGet = true)]
        public List<ClassMemberExtended> ClassMembers { get; set; } = new List<ClassMemberExtended>();
        public async Task<IActionResult> OnGetAsync(int? classID)
        {
            try
            {
                if (classID == null)
                {
                    return NotFound();
                }

                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    string url = string.Format("{0}/Meeting/GetClassMembers", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?classID={1}", url, classID);
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    ClassMembers = JsonSerializer.Deserialize<List<ClassMemberExtended>>(responseData, options);
                    if (ClassMembers == null)
                        ClassMembers = new List<ClassMemberExtended>();
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
