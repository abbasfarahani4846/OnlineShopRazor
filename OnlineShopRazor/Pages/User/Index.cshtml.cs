using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using OnlineShopRazor.Models.db;

using System.Security.Claims;

namespace OnlineShopRazor.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly OnlineShopContext _context;

        public IndexModel(OnlineShopContext context)
        {
            _context = context;
        }
        public OnlineShopRazor.Models.db.User UserInfo { get; set; }
        public void OnGet()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            UserInfo = _context.Users.FirstOrDefault(x => x.Id == userId);

        }

    }
}
