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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SM.APP.Services;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class EditModel(UserManager<IdentityUser> userManager, ILogger<EditModel> logger) : PageModelBase(userManager, logger)
    {
        [BindProperty]
        public Member Member { get; set; } = default!;

        public List<EventRegistration> EventRegistrations { get; set; } = new();
        public List<ClassMember> ClassMemberships { get; set; } = new();
        public List<ClassAttendance> ClassAttendances { get; set; } = new();
        public List<MemberAid> MemberAids { get; set; } = new();
        public List<Fund> Funds { get; set; } = new();


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
                    string url = string.Format("{0}/Member/GetMember", SMConfigurationManager.ApiBase);
                    string req = string.Format("{0}?memberID={1}", url, id);
                    HttpResponseMessage response = await client.GetAsync(req);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            MaxDepth = 128
                        };
                        Member member = JsonSerializer.Deserialize<Member>(responseData, options);
                        if (member == null)
                        {
                            return NotFound();
                        }
                        Member = member;

                        await LoadMemberActivities(id.Value, client, jwtToken, options);
                    }
                }
                return Page();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        private async Task LoadMemberActivities(int memberId, HttpClient client, string jwtToken, JsonSerializerOptions options)
        {
            try
            {
                // Load event registrations
                string eventUrl = $"{SMConfigurationManager.ApiBase}/Member/GetMemberEventRegistrations?memberID={memberId}";
                var eventResponse = await client.GetAsync(eventUrl);
                if (eventResponse.IsSuccessStatusCode)
                {
                    string responseData = await eventResponse.Content.ReadAsStringAsync();
                    EventRegistrations = JsonSerializer.Deserialize<List<EventRegistration>>(responseData, options) ?? new();
                }

                // Load class memberships
                string classUrl = $"{SMConfigurationManager.ApiBase}/Member/GetMemberClasses?memberID={memberId}";
                var classResponse = await client.GetAsync(classUrl);
                if (classResponse.IsSuccessStatusCode)
                {
                    string responseData = await classResponse.Content.ReadAsStringAsync();
                    ClassMemberships = JsonSerializer.Deserialize<List<ClassMember>>(responseData, options) ?? new();
                }

                // Load attendance history
                string attendanceUrl = $"{SMConfigurationManager.ApiBase}/Member/GetAttendanceHistory?memberID={memberId}";
                var attendanceResponse = await client.GetAsync(attendanceUrl);
                if (attendanceResponse.IsSuccessStatusCode)
                {
                    string responseData = await attendanceResponse.Content.ReadAsStringAsync();
                    ClassAttendances = JsonSerializer.Deserialize<List<ClassAttendance>>(responseData, options) ?? new();
                }

                // Load aids (optional)
                string aidUrl = $"{SMConfigurationManager.ApiBase}/Member/GetMemberAids?memberID={memberId}";
                var aidResponse = await client.GetAsync(aidUrl);
                if (aidResponse.IsSuccessStatusCode)
                {
                    string responseData = await aidResponse.Content.ReadAsStringAsync();
                    MemberAids = JsonSerializer.Deserialize<List<MemberAid>>(responseData, options) ?? new();
                }

                // Load funds (optional)
                string fundsUrl = $"{SMConfigurationManager.ApiBase}/Member/GetMemberFunds?memberID={memberId}";
                var fundsResponse = await client.GetAsync(fundsUrl);
                if (fundsResponse.IsSuccessStatusCode)
                {
                    string responseData = await fundsResponse.Content.ReadAsStringAsync();
                    Funds = JsonSerializer.Deserialize<List<Fund>>(responseData, options) ?? new();
                }
            }
            catch (Exception ex)
            {
                // ensuring the page still loads if some data can't be retrieved
                Console.WriteLine($"Error loading member activities: {ex.Message}");
            }
        } 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }


                var requestData = new
                {
                    memberID = Member.MemberID,
                    code = Member.Code,
                    unFirstName = Member.UNFirstName,
                    unLastName = Member.UNLastName,
                    nickname = Member.Nickname,
                    unFileNumber = Member.UNFileNumber,
                    unPersonalNumber = Member.UNPersonalNumber,
                    mobile = Member.Mobile,
                    baptised = Member.Baptised,
                    birthdate = Member.Birthdate,
                    gender = Member.Gender,
                    school = Member.School,
                    work = Member.Work,
                    isMainMember = Member.IsMainMember,
                    isActive = Member.IsActive,
                    imageReference = Member.ImageReference,
                    cardStatus = Member.CardStatus,
                    notes = Member.Notes,
                    imageURL = Member.ImageURL,
                    sequence = Member.Sequence
                };
                string request = string.Format("{0}/Member/UpdateMember", SMConfigurationManager.ApiBase);
                using (HttpClient client = new HttpClient())
                {
                    string jwtToken = await GetAPIToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
                    var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(request, jsonContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Error calling API: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }

                }


                return RedirectToPage("./Index", new { userCode = Member.Code });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
