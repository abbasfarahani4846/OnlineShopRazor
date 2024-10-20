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
        public void OnGet(string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                Products = _context.Products.Where(x =>
                        EF.Functions.Like(x.Title, "%" + searchText + "%") ||
                        EF.Functions.Like(x.Tags, "%" + searchText + "%")
                    )
                    .OrderBy(x => x.Title)
                    .ToList();
            }
            else
            {
                Products = _context.Products.OrderByDescending(x => x.Id).ToList();
            }
             
        }
    }
}
