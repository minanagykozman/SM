using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Classess
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModelBase
    {

        public IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : base(userManager, logger)
        {

        }
        [BindProperty(SupportsGet = true)]
        public List<Class> Classes { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    string req = string.Format("{0}/Meeting/GetClasses", SMConfigurationManager.ApiBase);
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    Classes = JsonSerializer.Deserialize<List<Class>>(responseData, options);
                    if (Classes == null)
                        Classes = new List<Class>();
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
