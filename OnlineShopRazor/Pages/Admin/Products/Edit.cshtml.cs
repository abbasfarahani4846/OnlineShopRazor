using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Products
{
    public class EditModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public EditModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public List<ProductGalery> Gallery { get; set; }
        public async Task<IActionResult> OnPostDeleteGallery(int id)
        {
            var gallery = await _context.ProductGaleries.FirstOrDefaultAsync(x => x.Id == id);

            if (gallery == null)
            {
                return NotFound();
            }

            // Delete the image file (if it exists)
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", gallery.ImageName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            // Remove the gallery entry from the database
            _context.ProductGaleries.Remove(gallery);
            await _context.SaveChangesAsync();

            return RedirectToPage("Edit", new { id = gallery.ProductId });
        }

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

            Product = product;

            Gallery = await _context.ProductGaleries.Where(x => x.ProductId == Product.Id).ToListAsync();
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
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

            _context.Attach(Product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(Product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
