using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Settings
{
    public class IndexModel : PageModel
    {
        private readonly OnlineShopRazor.Models.db.OnlineShopContext _context;

        public IndexModel(OnlineShopRazor.Models.db.OnlineShopContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Setting Setting { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {

            var setting = await _context.Settings.FirstOrDefaultAsync();
            if (setting == null)
            {
                return NotFound();
            }
            Setting = setting;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? newLogo)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (newLogo != null)
            {
                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\";

                string prevLogoPath = fn + Setting.Logo;

                if (System.IO.File.Exists(prevLogoPath))
                {
                    System.IO.File.Delete(prevLogoPath);
                }

                Setting.Logo = Guid.NewGuid() + Path.GetExtension(newLogo.FileName);
                //------------------------------------------------
                fn = fn + Setting.Logo;
                //------------------------------------------------

                using (var stream = new FileStream(fn, FileMode.Create))
                {
                    newLogo.CopyTo(stream);
                }

            }

            _context.Attach(Setting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(Setting.Id))
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

        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
