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
    public class DetailsModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public DetailsModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

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
    }
}
