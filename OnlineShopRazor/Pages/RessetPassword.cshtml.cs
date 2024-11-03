using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

using OnlineShopRazor.Models.db;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopRazor.Pages
{
    public class RessetPasswordModel : PageModel
    {
        private OnlineShopContext _context;
        [BindProperty, Required]
        public string? Email { get; set; }
        [BindProperty, Required]
        public string? NewPassword { get; set; }
        [BindProperty, Required]
        public int? RecoveryCode { get; set; }
        public RessetPasswordModel(OnlineShopContext context)
        {
            _context = context;
        }

        public void OnGet(string email)
        {
            Email = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ////-------------------------------------------

            var foundUser =await _context.Users.FirstOrDefaultAsync(x => x.Email == Email && x.RecoveryCode == RecoveryCode);
            if (foundUser == null)
            {
                ModelState.AddModelError("Email", "Email is not exist");
                return Page();
            }

            ////-------------------------------------------

            foundUser.Password = NewPassword;

            _context.Users.Update(foundUser);
            _context.SaveChanges();

            ////-------------------------------------------

            return RedirectToPage("Login");
        }
    }
}
