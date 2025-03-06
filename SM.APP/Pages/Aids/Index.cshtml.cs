using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Aids
{
    public class IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty(SupportsGet = true)]
        public List<Aid> Aids{ get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                string jwtToken = await GetAPIToken();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    string request = string.Format("{0}/Aids/GetAids", SMConfigurationManager.ApiBase);
                    HttpResponseMessage response = await client.GetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        Aids = JsonSerializer.Deserialize<List<Aid>>(responseData, options);
                    }
                }
                if (Aids == null)
                    Aids = new List<Aid>();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
