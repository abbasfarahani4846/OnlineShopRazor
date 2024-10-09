using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Products
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
        public Product Product { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? image, IFormFile[]? gallery)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (image != null)
            {
                Product.ImageName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                //------------------------------------------------
                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\products\\" + Product.ImageName;
                //------------------------------------------------

                using (var stream = new FileStream(fn, FileMode.Create))
                {
                    image.CopyTo(stream);
                }

            }
            //------------------------------------------------
            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            if (gallery != null)
            {
                foreach (var item in gallery)
                {
                    var newGallery = new ProductGalery();
                    newGallery.ProductId = Product.Id;
                    newGallery.ImageName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    //------------------------------------------------
                    string d = Directory.GetCurrentDirectory();
                    string fn = d + "\\wwwroot\\images\\products\\" + newGallery.ImageName;
                    //------------------------------------------------
                    using (var stream = new FileStream(fn, FileMode.Create))
                    {
                        item.CopyTo(stream);
                    }
                    //------------------------------------------------
                    _context.ProductGaleries.Add(newGallery);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
