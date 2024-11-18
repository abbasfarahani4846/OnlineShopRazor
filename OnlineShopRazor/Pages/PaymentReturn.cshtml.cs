using Ecommerce.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using OnlineShopRazor.Models.db;

using PayPal.Api;

namespace OnlineShopRazor.Pages
{
    public class PaymentReturnModel : PageModel
    {
        private OnlineShopContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;

        public PaymentReturnModel(OnlineShopContext context, IHttpContextAccessor httpContextAccessor,
            IConfiguration iconfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = iconfiguration;
            _context = context;
        }
        public int OrderId { get; set; }
        public IActionResult OnGet(int orderId=0, string PayerID="")
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return Page();
            }

            var clientId = _configuration.GetValue<string>("PayPal:Key");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");
            var apiContext = PaypalConfiguration.GetAPIContext(clientId, clientSecret, mode);

            try
            {
                var paymentId = _httpContextAccessor.HttpContext.Session.GetString("payment");
                var paymentExecution = new PaymentExecution { payer_id = PayerID };
                var payment = new Payment { id = paymentId };

                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() != "approved")
                {
                    return Page();
                }

                Response.Cookies.Delete("Cart");
                // Save the PayPal transaction ID and update order status
                order.TransId = executedPayment.transactions[0].related_resources[0].sale.id;
                order.Status = executedPayment.state.ToLower();

                //---------Reduce QTY-------------
                var orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId).ToList();
                var productsIds = orderDetails.Select(x => x.ProductId);

                var products = _context.Products.Where(x => productsIds.Contains(x.Id)).ToList();

                foreach (var item in products)
                {
                    item.Qty -= orderDetails.FirstOrDefault(x => x.ProductId == item.Id).Count;
                }

                _context.Products.UpdateRange(products);
                //----------------------
                _context.SaveChanges();

                OrderId = order.Id;

                return Page();
            }
            catch (Exception)
            {
                return Page();
            }
        }
    }
}
