using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public IndexModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        public IList<Banner> Banner { get; set; } = default!;
        public IList<Product> NewProducts { get; set; } = default!;
        public IList<BestSellingFinal> BestSellingProducts { get; set; } = default!;
        public async Task OnGetAsync()
        {
            Banner = await _context.Banners.ToListAsync();
            //----------------------------------------------------
            NewProducts =_context.Products.OrderByDescending(x=>x.Id).Take(8).ToList();
            //----------------------------------------------------
            BestSellingProducts = _context.BestSellingFinals.ToList();
            //----------------------------------------------------
        }

    }
}
