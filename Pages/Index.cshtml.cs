// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Dealership.Models;

namespace Dealership.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly DatabaseService _db;
        public List<SaleData> Sales { get; set; } = new();

        public IndexModel(DatabaseService db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            var dealerCode = User.FindFirst("DealerCode")?.Value;
            if (!string.IsNullOrEmpty(dealerCode))
            {
                Sales = await _db.GetSalesForDealerAsync(dealerCode);
            }
        }
    }
}