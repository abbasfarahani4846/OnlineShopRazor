using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Coupons
{
    public class DeleteModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public DeleteModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Coupon Coupon { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FirstOrDefaultAsync(m => m.Id == id);

            if (coupon == null)
            {
                return NotFound();
            }
            else
            {
                Coupon = coupon;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                Coupon = coupon;
                _context.Coupons.Remove(Coupon);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
