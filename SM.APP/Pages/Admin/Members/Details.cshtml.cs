using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.DAL.DataModel;
using SM.BAL;
using SM.APP.Services;
using Microsoft.AspNetCore.Identity;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Policy = "Members.View")]
    public class DetailsModel(ILogger<DetailsModel> logger) : PageModelBase(logger)
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

                using (var memberHandler = new MemberHandler())
                {
                    // Get member details
                    Member = memberHandler.GetMember(id.Value, false);
                    if (Member == null)
                    {
                        return NotFound();
                    }

                    // Get event registrations
                    var eventRegistrations = memberHandler.GetMemberEventRegistrations(id.Value);
                    if (eventRegistrations != null)
                    {
                        EventRegistrations = eventRegistrations.Cast<EventRegistration>().ToList();
                    }

                    // Get class memberships
                    var classMembers = memberHandler.GetMemberClasses(id.Value);
                    if (classMembers != null)
                    {
                        ClassMembers = classMembers.Cast<ClassMember>().ToList();
                    }

                    // Get attendance statistics
                    var attendanceStats = memberHandler.GetMemberClassOverviews(id.Value);
                    if (attendanceStats != null)
                    {
                        ClassAttendanceStats = attendanceStats.Cast<MemberClassOverview>().ToList();
                    }

                    // Get member aids
                    var memberAids = memberHandler.GetMemberAids(id.Value);
                    if (memberAids != null)
                    {
                        MemberAids = memberAids;
                    }

                    // Get member funds
                    var memberFunds = memberHandler.GetMemberFunds(id.Value);
                    if (memberFunds != null)
                    {
                        Funds = memberFunds;
                    }

                    var familyMembers = memberHandler.GetFamilyByUNFileNumber(Member.UNFileNumber);
                    FamilyMembers = familyMembers?
                        .Where(m => m?.MemberID != Member.MemberID)
                        .ToList() ?? new List<Member>();
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
