using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Coupons
{
    public class CreateModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public CreateModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Coupon Coupon { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Coupons.Add(Coupon);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
