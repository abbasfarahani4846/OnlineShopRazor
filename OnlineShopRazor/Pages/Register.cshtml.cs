using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public RegisterModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(User.Email);
            if (!match.Success)
            {
                ModelState.AddModelError("User.Email", "Email is not valid");
                return Page();
            }

            User.Email = User.Email.Trim();
            var prevUser = _context.Users.Any(x => x.Email == User.Email);
            if (prevUser)
            {
                ModelState.AddModelError("User.Email", "Email is used");
                return Page();
            }

            User.RegisterDate = DateTime.Now;
            User.IsAdmin = false;
            User.Password = User.Password.Trim();
            User.FullName = User.FullName.Trim();
            User.RecoveryCode = 0;
            
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/login");
            
        }
    }
}
