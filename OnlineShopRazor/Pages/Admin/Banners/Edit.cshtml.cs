using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Banners
{
    public class EditModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public EditModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Banner Banner { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner =  await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);
            if (banner == null)
            {
                return NotFound();
            }
            Banner = banner;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? newImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (newImage != null)
            {
                //حذف عکس اصلی
                if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName))
                {
                    System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName);
                }


                Banner.ImageName = Guid.NewGuid() + Path.GetExtension(newImage.FileName);

                //ذخیره تصویر اصلی
                string ImagePath = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName;

                using (var stream = new FileStream(ImagePath, FileMode.Create))
                {
                    newImage.CopyTo(stream);
                }

            }

            _context.Attach(Banner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(Banner.Id))
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

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
