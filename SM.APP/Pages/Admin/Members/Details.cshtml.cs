using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using SM.APP.Services;
using Microsoft.AspNetCore.Identity;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class DetailsModel(UserManager<IdentityUser> userManager, ILogger<DetailsModel> logger) : PageModelBase(userManager, logger)
    {
        public Member? Member { get; set; }

        public List<EventRegistration> EventRegistrations { get; set; } = new();
        public List<ClassMember> ClassMembers { get; set; } = new();
        public List<MemberClassOverview> ClassAttendanceStats { get; set; } = new();
        public List<MemberAid> MemberAids { get; set; } = new();
        public List<MemberFund> Funds { get; set; } = new();
        public List<Member> FamilyMembers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Get member details
                    string memberUrl = string.Format("{0}/Member/GetMember?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage memberResponse = await client.GetAsync(memberUrl);
                    if (memberResponse.IsSuccessStatusCode)
                    {
                        string memberData = await memberResponse.Content.ReadAsStringAsync();
                        Member = JsonSerializer.Deserialize<Member>(memberData, options);
                    }

                    if (Member == null)
                    {
                        return NotFound();
                    }

                    // Get event registrations
                    string eventRegistrationsUrl = string.Format("{0}/Member/GetMemberEventRegistrations?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage eventRegistrationsResponse = await client.GetAsync(eventRegistrationsUrl);
                    if (eventRegistrationsResponse.IsSuccessStatusCode)
                    {
                        string eventRegistrationsData = await eventRegistrationsResponse.Content.ReadAsStringAsync();
                        var eventRegistrations = JsonSerializer.Deserialize<List<EventRegistration>>(eventRegistrationsData, options);
                        if (eventRegistrations != null)
                        {
                            EventRegistrations = eventRegistrations;
                        }
                    }

                    // Get class memberships
                    string classMembersUrl = string.Format("{0}/Member/GetMemberClasses?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage classMembersResponse = await client.GetAsync(classMembersUrl);
                    if (classMembersResponse.IsSuccessStatusCode)
                    {
                        string classMembersData = await classMembersResponse.Content.ReadAsStringAsync();
                        var classMembers = JsonSerializer.Deserialize<List<ClassMember>>(classMembersData, options);
                        if (classMembers != null)
                        {
                            ClassMembers = classMembers;
                        }
                    }

                    // Get attendance statistics
                    string attendanceStatsUrl = string.Format("{0}/Member/GetMemberClassOverview?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage attendanceStatsResponse = await client.GetAsync(attendanceStatsUrl);
                    if (attendanceStatsResponse.IsSuccessStatusCode)
                    {
                        string attendanceStatsData = await attendanceStatsResponse.Content.ReadAsStringAsync();
                        var attendanceStats = JsonSerializer.Deserialize<List<MemberClassOverview>>(attendanceStatsData, options);
                        if (attendanceStats != null)
                        {
                            ClassAttendanceStats = attendanceStats;
                        }
                    }

                    // Get member aids
                    string memberAidsUrl = string.Format("{0}/Member/GetMemberAids?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage memberAidsResponse = await client.GetAsync(memberAidsUrl);
                    if (memberAidsResponse.IsSuccessStatusCode)
                    {
                        string memberAidsData = await memberAidsResponse.Content.ReadAsStringAsync();
                        var memberAids = JsonSerializer.Deserialize<List<MemberAid>>(memberAidsData, options);
                        if (memberAids != null)
                        {
                            MemberAids = memberAids;
                        }
                    }

                    // Get member funds
                    string memberFundsUrl = string.Format("{0}/Member/GetMemberFunds?memberID={1}", SMConfigurationManager.ApiBase, id.Value);
                    HttpResponseMessage memberFundsResponse = await client.GetAsync(memberFundsUrl);
                    if (memberFundsResponse.IsSuccessStatusCode)
                    {
                        string memberFundsData = await memberFundsResponse.Content.ReadAsStringAsync();
                        var memberFunds = JsonSerializer.Deserialize<List<MemberFund>>(memberFundsData, options);
                        if (memberFunds != null)
                        {
                            Funds = memberFunds;
                        }
                    }

                    // Get family members
                    if (!string.IsNullOrEmpty(Member.UNFileNumber))
                    {
                        string familyUrl = string.Format("{0}/Member/GetFamily?unFileNumber={1}", SMConfigurationManager.ApiBase, Member.UNFileNumber);
                        HttpResponseMessage familyResponse = await client.GetAsync(familyUrl);
                        if (familyResponse.IsSuccessStatusCode)
                        {
                            string familyData = await familyResponse.Content.ReadAsStringAsync();
                            var familyMembers = JsonSerializer.Deserialize<List<Member>>(familyData, options);
                            FamilyMembers = familyMembers?
                                .Where(m => m?.MemberID != Member.MemberID)
                                .ToList() ?? new List<Member>();
                        }
                    }
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
