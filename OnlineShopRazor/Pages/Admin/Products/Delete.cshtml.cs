using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Products
{
    public class DeleteModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public DeleteModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\products\\";

                string mainImagePath = fn + product.ImageName;

                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(fn);
                }

                var galleries = _context.ProductGaleries.Where(x => x.ProductId == id).ToList();
                if (galleries != null)
                {
                    foreach (var item in galleries)
                    {
                        string galleryImagePath = fn + item.ImageName;

                        if (System.IO.File.Exists(galleryImagePath))
                        {
                            System.IO.File.Delete(galleryImagePath);
                        }
                    }

                    _context.ProductGaleries.RemoveRange(galleries);
                }

                Product = product;
                _context.Products.Remove(Product);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
