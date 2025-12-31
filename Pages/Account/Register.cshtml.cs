// Pages/Account/Register.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Dealership.Models;

namespace Dealership.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly DatabaseService _db;

        [BindProperty]
        public string FullName { get; set; } = "";

        [BindProperty]
        public string DealerCode { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        public string ErrorMessage { get; set; } = "";

        public RegisterModel(DatabaseService db)
        {
            _db = db;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Пароли не совпадают";
                return Page();
            }

            var existingUser = await _db.GetUserByDealerCodeAsync(DealerCode);
            if (existingUser != null)
            {
                ErrorMessage = "Пользователь с таким кодом уже существует";
                return Page();
            }

            var user = new User
            {
                DealerCode = DealerCode,
                FullName = FullName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Password),
                IsAdmin = false
            };

            await _db.CreateUserAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("DealerCode", user.DealerCode),
                new Claim("IsAdmin", "false")
            };

            var identity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToPage("/Index");
        }
    }
}