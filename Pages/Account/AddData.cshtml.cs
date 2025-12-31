// Pages/Account/AddData.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Dealership.Models;

namespace Dealership.Pages.Account
{
    [Authorize]
    public class AddDataModel : PageModel
    {
        private readonly DatabaseService _db;

        [BindProperty]
        public int Month { get; set; }

        [BindProperty]
        public int Year { get; set; } = DateTime.Now.Year;

        [BindProperty]
        public int BrandId { get; set; }

        [BindProperty]
        public int ModelId { get; set; }

        [BindProperty]
        public int CarsSold { get; set; }

        [BindProperty]
        public int CancelledSales { get; set; }

        [BindProperty]
        public int CarsDelivered { get; set; }

        public string Message { get; set; } = "";
        public bool IsSuccess { get; set; }

        public AddDataModel(DatabaseService db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var dealerCode = User.FindFirst("DealerCode")?.Value;

            if (string.IsNullOrEmpty(dealerCode))
            {
                Message = "Ошибка: пользователь не авторизован";
                return Page();
            }

            var monthDate = new DateTime(Year, Month, 1);

            await _db.AddSaleDataAsync(dealerCode, monthDate, BrandId, ModelId,
                                      CarsSold, CancelledSales, CarsDelivered);

            Message = "Данные успешно добавлены!";
            IsSuccess = true;

            return Page();
        }
    }
}