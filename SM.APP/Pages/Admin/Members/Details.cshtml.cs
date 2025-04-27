using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Members
{
    [Authorize(Roles = "Admin,Servant")]
    public class DetailsModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public DetailsModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public Member? Member { get; set; }

        public List<EventRegistration> EventRegistrations { get; set; } = new();
        public List<ClassMember> ClassMembers { get; set; } = new();
        public List<MemberClassOverview> ClassAttendanceStats { get; set; } = new();
        public List<MemberAid> MemberAids { get; set; } = new();
        public List<Fund> Funds { get; set; } = new();
        public List<Member> FamilyMembers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Member = await _context.Members
                .Include(m => m.EventRegistrations)
                    .ThenInclude(er => er.Event)
                .Include(m => m.ClassMembers)
                    .ThenInclude(cm => cm.Class)
                .Include(m => m.MemberAids)
                    .ThenInclude(ma => ma.Aid)
                .Include(m => m.MemberAids)
                    .ThenInclude(ma => ma.Servant)
                .Include(m => m.Funds)
                    .ThenInclude(f => f.Aid)
                .Include(m => m.Funds)
                    .ThenInclude(f => f.Servant)
                .FirstOrDefaultAsync(m => m.MemberID == id);

            if (Member == null)
            {
                return NotFound();
            }

            // Set the collections from the loaded Member
            EventRegistrations = Member.EventRegistrations.ToList();
            ClassMembers = Member.ClassMembers.ToList();
            MemberAids = Member.MemberAids.ToList();
            Funds = Member.Funds.ToList();

            // Get attendance statistics
            List<MemberClassOverview> stats;
            using (var memberHandler = new SM.BAL.MemberHandler())
            {
                stats = memberHandler.GetMemberClassses(id.Value);
            }
            ClassAttendanceStats = stats;

            FamilyMembers = await _context.Members
                .Where(m => m.UNFileNumber == Member.UNFileNumber && m.MemberID != Member.MemberID)
                .OrderByDescending(m => m.Birthdate)
                .ToListAsync();

            return Page();
        }
    }
}
