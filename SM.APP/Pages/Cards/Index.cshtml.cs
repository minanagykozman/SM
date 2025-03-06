using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SM.APP.Services;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Cards
{
    [Authorize(Roles = "Admin,Servant")]
    public class IndexModel : PageModelBase
    {
        public IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger) : base(userManager, logger)
        {
        }
        public bool ShowModal { get; set; } = false;
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string CardStatus { get; set; } = string.Empty;
        [BindProperty(SupportsGet = true)]
        public string MemberStatus { get; set; } = string.Empty;
        public Member Member { get; set; } = default!;
        public IList<Member> Members { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(UserCode))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string jwtToken = await GetAPIToken();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        string url = string.Format("{0}/Member/GetMemberByCode", SMConfigurationManager.ApiBase);
                        string req = string.Format("{0}?memberCode={1}", url, UserCode);
                        HttpResponseMessage response = await client.GetAsync(req);
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Enable case insensitivity
                        };
                        if (!string.IsNullOrEmpty(responseData))
                            Member = JsonSerializer.Deserialize<Member>(responseData, options);
                        ShowModal = true;
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
                        Members = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                        if (Members == null)
                            Members = new List<Member>();
                    }

                    UserCode = string.Empty;
                }
                if (Members == null)
                    Members = new List<Member>();
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
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
                return RedirectToPage("", new { cardStatus = CardStatus });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
