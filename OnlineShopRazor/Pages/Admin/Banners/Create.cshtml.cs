using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

using OnlineShopRazor.Models.db;

namespace OnlineShopRazor.Pages.Admin.Banners
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
        public Banner Banner { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? newImage)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (newImage != null)
            {
                Banner.ImageName = Guid.NewGuid() + Path.GetExtension(newImage.FileName);

                //ذخیره تصویر اصلی
                string ImagePath = Directory.GetCurrentDirectory() + "\\wwwroot\\images\\banners\\" + Banner.ImageName;

                using (var stream = new FileStream(ImagePath, FileMode.Create))
                {
                    newImage.CopyTo(stream);
                }

            }

            _context.Banners.Add(Banner);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
