using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SM.DAL;
using SM.DAL.DataModel;

namespace SM.APP.Pages.Admin.Class
{
    public class IndexModel : PageModel
    {
        private readonly SM.DAL.AppDbContext _context;

        public IndexModel(SM.DAL.AppDbContext context)
        {
            _context = context;
        }

        public IList<DAL.DataModel.Class> Class { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Class = await _context.Classes
                .Include(c => c.Meeting).ToListAsync();
        }
    }
}
