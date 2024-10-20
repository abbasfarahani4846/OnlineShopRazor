using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages;

public class ProductDetails : PageModel
{
    private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

    public ProductDetails(OnlineShopRazor.Models.db.OnlineShopContext context)
    {
        _context = context;
    }

    public Product? Product { get; set; }
    public List<ProductGalery> ProductGaleries { get; set; } = new List<ProductGalery>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Product> NewProducts { get; set; } = new List<Product>();
    public IActionResult OnGet(int id)
    {
        
        Product = _context.Products.FirstOrDefault(x => x.Id == id);

        if (Product == null)
        {
            return NotFound();
        }

        ProductGaleries = _context.ProductGaleries.Where(x => x.ProductId == id).ToList();

        NewProducts = _context.Products.Where(x => x.Id != id).Take(6).OrderByDescending(x => x.Id).ToList();

        Comments  = _context.Comments.Where(x => x.ProductId == id).OrderByDescending(x => x.CreateDate).ToList();
        return Page();

    }
}