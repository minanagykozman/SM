using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SM.APP.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        public JsonResult OnPostMyAjaxHandler()
        {
            // Logic to process the AJAX request
            var response = new
            {
                message = "Button clicked successfully via AJAX!"
            };

            return new JsonResult(response);
        }
        public string Message { get; set; }
        public void OnPostMyButtonClick()
        {
            // Code to handle button click.
            Message = "";
        }
    }
}
