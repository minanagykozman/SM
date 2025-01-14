
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
    public class IndexModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public IndexModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<DAL.DataModel.Meeting> Meeting { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Meeting = await _context.Meetings.ToListAsync();
        }
    }
}
