using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SM.APP.Pages
{
    [Authorize]
    public class RefreshTokenModel : PageModel
    {
        // This endpoint's only job is to be called.
        // It triggers the middleware pipeline, which does the real work.
        // Returning NoContent is efficient as no response body is sent.
        public IActionResult OnGet()
        {
            return new NoContentResult(); // Returns a 204 No Content response
        }
    }
}
