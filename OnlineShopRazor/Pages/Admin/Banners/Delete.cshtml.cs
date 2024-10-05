using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Banners
{
    public class DeleteModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public DeleteModel(OnlineShopRazor.Models.db.OnlineShopContext context)
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

            var banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

            if (banner == null)
            {
                return NotFound();
            }
            else
            {
                Banner = banner;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);

            if (banner != null)
            {
                Banner = banner;

              
                //حذف عکس اصلی
                if (System.IO.File.Exists(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName))
                {
                    System.IO.File.Delete(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName);
                }

                _context.Banners.Remove(Banner);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
