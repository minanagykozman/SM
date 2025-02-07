using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Text;
using System.Text.Json;

namespace SM.APP.Pages.Attendance
{
    [Authorize(Roles = "Admin,Servant")]
    public class TakeAttendanceModel : PageModel
    {
        int _sevantID = 0;
        private readonly UserManager<IdentityUser> _userManager;
        public TakeAttendanceModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;

        }
        [BindProperty(SupportsGet = true)]
        public string UserCode { get; set; } = string.Empty;
        [BindProperty]
        public string? Notes { get; set; } = null;
        [BindProperty]
        public int ClassOccurenceID { get; set; }
        public string MemberStatus { get; set; } = string.Empty;

        public bool ShowModal { get; set; } = false;
        public static MemberAttendanceResult? MemberData { get; set; }
        public List<Member> ClassOccurenceMembers { get; set; } = new List<Member>();

        public async Task OnGetAsync(int? classOccurenceID)
        {
            if (_sevantID == 0)
            {
                var userId = _userManager.GetUserId(User);
                ServantService service = new ServantService();
                _sevantID = service.GetServantID(userId);
            }
            if (classOccurenceID.HasValue)
            {
                ClassOccurenceID = classOccurenceID.Value;
                if (!string.IsNullOrEmpty(UserCode))
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string url = "https://apitest.stmosesservices.com/Meeting/CheckAttendance";
                        string req = string.Format("{0}?classOccurenceID={1}&memberCode={2}", url, classOccurenceID, UserCode);
                        HttpResponseMessage response = await client.GetAsync(req);
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true // Enable case insensitivity
                        };
                        if (!string.IsNullOrEmpty(responseData))
                            MemberData = JsonSerializer.Deserialize<MemberAttendanceResult>(responseData, options);
                    }
                    if (MemberData != null)
                    {
                        if (MemberData.AttendanceStatus == AttendanceStatus.Ready)
                        {
                            StringContent jsonContent;
                            string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, false, out jsonContent);
                            using (HttpClient client = new HttpClient())
                            {
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
                    string req = "https://apitest.stmosesservices.com/Meeting/GetAttendedMembers?occurenceID=" + ClassOccurenceID.ToString();
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
            if (_sevantID == 0)
            {
                var userId = _userManager.GetUserId(User);
                ServantService service = new ServantService();
                _sevantID = service.GetServantID(userId);
            }
            if (MemberData != null)
            {
                StringContent jsonContent;
                string request = TakeAttendanceString(MemberData.Member.Code, ClassOccurenceID, true, out jsonContent);
                using (HttpClient client = new HttpClient())
                {

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

        private string TakeAttendanceString(string memberCode, int classOccuranceID, bool forceRegister, out StringContent jsonContent)
        {
            int servantID = _sevantID;
            var requestData = new
            {
                classOccuranceID,
                memberCode,
                servantID,
                forceRegister
            };

            string apiUrl = "https://apitest.stmosesservices.com/Meeting/TakeAttendance";
            string request = string.Format("{0}?classOccurenceID={1}&memberCode={2}&servantID={3}&forceRegister={4}", apiUrl, ClassOccurenceID, memberCode, _sevantID, forceRegister.ToString().ToLower());
            jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            return request;
        }
    }
    public class MemberAttendanceResult
    {
        public AttendanceStatus AttendanceStatus { get; set; }
        public Member Member { get; set; }
    }
    public enum AttendanceStatus
    {
        NotRegisteredInClass,
        MemberNotFound,
        ClassNotFound,
        AlreadyAttended,
        Ready,
        Ok
    }
}
