using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
namespace OnlineShopRazor.Pages
{
    public class LoginModel : PageModel
    {
        private OnlineShopContext _context;
        [BindProperty, Required]
        public string? Username { get; set; }
        [BindProperty, DataType(DataType.Password)]
        public string? Password { get; set; }
        public LoginModel(OnlineShopContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var findUser = _context.Users.FirstOrDefault(x => x.Email == Username.Trim() && x.Password == Password.Trim());
            if (findUser == null)
            {
                ModelState.AddModelError("Username", "User not exists");
                return Page();
            }

            // Create claims for the authenticated user
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, findUser.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, findUser.FullName));
            claims.Add(new Claim(ClaimTypes.Email, findUser.Email));
            claims.Add(new Claim(ClaimTypes.Role, findUser.IsAdmin ? "admin" : "user"));

            // Create an identity based on the claims
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Create a principal based on the identity
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user with the created principal
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Redirect("/");

        }
    }
}
