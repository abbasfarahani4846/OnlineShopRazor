using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages
{
    public class ShopModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public ShopModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        public List<Product> Products { get; set; }
        public void OnGet()
        {
             Products = _context.Products.OrderByDescending(x => x.Id).ToList();
        }
    }
}
