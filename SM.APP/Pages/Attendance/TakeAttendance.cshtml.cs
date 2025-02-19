using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    [Authorize(Roles = "Admin,Servant")]
    public class TakeAttendanceModel : PageModelBase
    {
        public TakeAttendanceModel(UserManager<IdentityUser> userManager, ILogger<TakeAttendanceModel> logger) : base(userManager, logger)
        {

        }
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty]
        public string? Notes { get; set; } = null;
        [BindProperty]
        public int ClassOccurenceID { get; set; }
        public string MemberStatus { get; set; } = string.Empty;

        public bool ShowModal { get; set; } = false;
        public MemberAttendanceResult? MemberData { get; set; }
        public List<Member> ClassOccurenceMembers { get; set; } = new List<Member>();

        public async Task<IActionResult> OnGetAsync(int? classOccurenceID)
        {
            try
            {
                if (classOccurenceID.HasValue)
                {
                    ClassOccurenceID = classOccurenceID.Value;
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string jwtToken = await GetAPIToken();
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                            string url = string.Format("{0}/Meeting/CheckAttendance", SMConfigurationManager.ApiBase);
                            string req = string.Format("{0}?classOccurenceID={1}&memberCode={2}", url, classOccurenceID, UserCode);
                            HttpResponseMessage response = await client.GetAsync(req);
                            string responseData = await response.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Enable case insensitivity
                            };
                            if (!string.IsNullOrEmpty(responseData))
                            {
                                MemberData = JsonSerializer.Deserialize<MemberAttendanceResult>(responseData, options);
                                HttpContext.Session.SetString("MemberAttendanceData", JsonSerializer.Serialize(MemberData));
                            }
                        }
                        if (MemberData != null)
                        {
                            if (MemberData.AttendanceStatus == AttendanceStatus.Ready)
                            {
                                StringContent jsonContent;
                                string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, false, out jsonContent);
                                using (HttpClient client = new HttpClient())
                                {
                                    string jwtToken = await GetAPIToken();
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                                    HttpResponseMessage response = await client.PostAsync(request, jsonContent);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        string responseData = await response.Content.ReadAsStringAsync();
                                    }
                                    else
                                    {
                                        throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                                    }
                                    UserCode = string.Empty;
                                }

                            }
                            LoadData();
                        }
                        UserCode = string.Empty;
                    }
                    using (HttpClient client = new HttpClient())
                    {
                        string jwtToken = await GetAPIToken();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        string url = string.Format("{0}/Meeting/GetAttendedMembers", SMConfigurationManager.ApiBase);
                        string req = string.Format("{0}?occurenceID={1}", url, ClassOccurenceID.ToString());
                        HttpResponseMessage response = await client.GetAsync(req);
                        string responseData = await response.Content.ReadAsStringAsync();
                        if (!string.IsNullOrEmpty(responseData))
                        {
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true // Enable case insensitivity
                            };
                            ClassOccurenceMembers = JsonSerializer.Deserialize<List<Member>>(responseData, options);
                        }
                        if (ClassOccurenceMembers == null)
                            ClassOccurenceMembers = new List<Member>();
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private void LoadData()
        {
            switch (MemberData.AttendanceStatus)
            {
                case AttendanceStatus.MemberNotFound:
                    MemberStatus = "Member not found";
                    ShowModal = true;
                    break;
                case AttendanceStatus.ClassNotFound:
                    MemberStatus = "Class not found";
                    ShowModal = true;
                    break;
                case AttendanceStatus.AlreadyAttended:
                    MemberStatus = "Member already had attendance";
                    ShowModal = true;
                    break;
                case AttendanceStatus.NotRegisteredInClass:
                    MemberStatus = "Member not registered in class";
                    ShowModal = true;
                    break;
                case AttendanceStatus.Ready:
                    MemberStatus = "Can Attend";
                    ShowModal = false;
                    break;
                default:
                    break;
            }


        }
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var sessionData = HttpContext.Session.GetString("MemberAttendanceData");
                if (!string.IsNullOrEmpty(sessionData))
                {
                    MemberData = JsonSerializer.Deserialize<MemberAttendanceResult>(sessionData);
                }
                if (MemberData != null)
                {
                    StringContent jsonContent;
                    string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, true, out jsonContent);
                    using (HttpClient client = new HttpClient())
                    {
                        string jwtToken = await GetAPIToken();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                        HttpResponseMessage response = await client.PostAsync(request, jsonContent);
                        if (response.IsSuccessStatusCode)
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                        }
                        UserCode = string.Empty;
                    }
                }
                return RedirectToPage("", new { classOccurenceID = ClassOccurenceID });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private string TakeAttendanceString(string memberCode, int classOccuranceID, bool forceRegister, out StringContent jsonContent)
        {
            var requestData = new
            {
                classOccuranceID,
                memberCode,
                forceRegister
            };
            string url = string.Format("{0}/Meeting/TakeAttendance", SMConfigurationManager.ApiBase);
            string request = string.Format("{0}?classOccurenceID={1}&memberCode={2}&forceRegister={3}", url, ClassOccurenceID, memberCode, forceRegister.ToString().ToLower());
            jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            return request;
        }
    }
    public class MemberAttendanceResult
    {
        public AttendanceStatus AttendanceStatus { get; set; }
        public Member Member { get; set; }
    }
}
