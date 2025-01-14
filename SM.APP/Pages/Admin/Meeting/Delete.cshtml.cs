using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Meeting
{
    public class DeleteModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public DeleteModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DAL.DataModel.Meeting Meeting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.MeetingID == id);

            if (meeting == null)
            {
                return NotFound();
            }
            else
            {
                Meeting = meeting;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                Meeting = meeting;
                _context.Meetings.Remove(Meeting);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
