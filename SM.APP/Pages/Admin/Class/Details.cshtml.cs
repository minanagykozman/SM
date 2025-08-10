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

namespace SM.APP.Pages.Admin.Class
{
    [Authorize(Policy = "Class.Manage")]
    public class DetailsModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public DetailsModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public SM.DAL.DataModel.Class Class { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cl = await _context.Classes.FirstOrDefaultAsync(m => m.ClassID == id);
            if (cl == null)
            {
                return NotFound();
            }
            else
            {
                Class = cl;
            }
            return Page();
        }
    }
}
