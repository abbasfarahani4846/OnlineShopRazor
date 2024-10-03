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
    public class IndexModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public IndexModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        public IList<Coupon> Coupon { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Coupon = await _context.Coupons.ToListAsync();
        }
    }
}
