using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SM.APP.Services;
using SM.DAL.DataModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SM.BAL;

namespace SM.APP.Pages.Funds
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        private readonly FundHandler _fundHandler;
        private readonly IConfiguration _configuration;

        public List<MemberFund> Funds { get; set; } = new();
        public List<object> AssignableServants { get; set; } = new();
        public List<string> FundStatuses { get; set; } = new() { "Open", "Approved", "Rejected", "Delivered" };
        public List<string> FundCategories { get; set; } = new() { "Others", "Medical", "Education", "Emergency" };

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AssigneeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public string ErrorMessage { get; set; }

        public IndexModel(UserManager<IdentityUser> userManager, ILogger<IndexModel> logger, IConfiguration configuration) : base(userManager, logger)
        {
            _configuration = configuration;
            _fundHandler = new FundHandler();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Ensure API token is set for JavaScript calls
                await GetAPIToken();
                
                var username = User.Identity?.Name ?? "";
                
                // Load funds with filters
                Funds = _fundHandler.GetAllFunds(AssigneeFilter, StatusFilter, username);

                // Load assignable servants for dropdowns
                AssignableServants = _fundHandler.GetAssignableServants();

                // Apply search filter
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Funds = Funds.Where(f => f.Member.FullName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                           f.RequestDescription.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading funds: " + ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCreateFundAsync(int memberID, string requestDescription, int servantID, decimal requestedAmount, string fundCategory)
        {
            try
            {
                var username = User.Identity?.Name ?? "";
                
                var request = new CreateFundRequest
                {
                    MemberID = memberID,
                    RequestDescription = requestDescription,
                    ServantID = servantID,
                    RequestedAmount = requestedAmount,
                    FundCategory = fundCategory
                };

                var fundId = _fundHandler.CreateFund(request, username);
                TempData["SuccessMessage"] = "Fund request created successfully.";
                
                return RedirectToPage("./Detail", new { id = fundId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating fund: {ex.Message}";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostDeleteFundAsync(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "";
                _fundHandler.DeleteFund(id, username);
                TempData["SuccessMessage"] = "Fund deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting fund: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
} 