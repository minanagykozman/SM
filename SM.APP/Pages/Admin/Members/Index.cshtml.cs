using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        

        public IList<Member> Member { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(UserCode))
            {
                using (HttpClient client = new HttpClient())
                {
                    string req = "https://apitest.stmosesservices.com/Events/CheckRegistrationStatus?memberCode=" + UserCode;
                    HttpResponseMessage response = await client.GetAsync(req);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Enable case insensitivity
                    };
                    Member = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                }
                UserCode = string.Empty;
            }
        }
    }
}
