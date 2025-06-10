using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.APP.Services;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string FirstName { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string LastName { get; set; } = string.Empty;


        public IList<Member> Member { get; set; } = default!;

        public async Task OnGetAsync(string userCode)
        {
            if (!string.IsNullOrEmpty(userCode))
            {
                UserCode = userCode;
            }
            if (!string.IsNullOrEmpty(UserCode)|| !string.IsNullOrEmpty(FirstName) || !string.IsNullOrEmpty(LastName))
            {
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    string url = string.Format("{0}/Member/SearchMembers", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?memberCode={1}&firstName={2}&lastName={3}", url,UserCode, FirstName, LastName);
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    Member = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                    if (Member == null)
                        Member = new List<Member>();
                }

                UserCode = string.Empty;
            }
            if (Member == null)
                Member = new List<Member>();
        }
    }
}
