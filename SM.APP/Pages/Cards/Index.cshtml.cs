using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
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
using static Azure.Core.HttpHeader;

namespace SM.APP.Pages.Cards
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModelBase
    {
        public IndexModel(UserManager<IdentityUser> userManager) : base(userManager)
        {
        }
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string CardStatus { get; set; } = string.Empty;
        public IList<Member> Member { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (!string.IsNullOrEmpty(UserCode) && !string.IsNullOrEmpty(CardStatus))
            {
                CardStatus cardStatus = (CardStatus)Enum.Parse(typeof(CardStatus), CardStatus);
                var requestData = new
                {
                    memberCode = UserCode,
                    cardStatus = cardStatus
                };
                string request = string.Format("{0}/Member/UpdateMemberCard", SMConfigurationManager.ApiBase);
                //string request = string.Format("{0}?memberCode={1}&eventID={2}&&isException={3}&notes={4}", url, memberCode, EventID, isException.ToString().ToLower(), notes);
                string jwtToken = await GetAPIToken();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(request, jsonContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                    UserCode = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(CardStatus))
            {
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    string url = string.Format("{0}/Member/GetMembersByCardStatus", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?cardStatus={1}", url, CardStatus);
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
