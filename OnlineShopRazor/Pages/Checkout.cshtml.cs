using Ecommerce.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using OnlineShopRazor.Models.db;
using OnlineShopRazor.Models.ViewModels;

using PayPal.Api;

using System.Linq;
using System.Security.Claims;

namespace OnlineShopRazor.Pages
{
    [Authorize]
    public class CheckoutModel : PageModel
    {
        private OnlineShopContext _context;
        private IHttpContextAccessor _httpContextAccessor;
        IConfiguration _configuration;

        public CheckoutModel(OnlineShopContext context, IHttpContextAccessor httpContextAccessor,
            IConfiguration iconfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = iconfiguration;
            _context = context;
        }
        [BindProperty]
        public List<ProductCartViewModel> ProductCart { get; set; }
        [BindProperty]
        public Models.db.Order Order { get; set; } = new Models.db.Order();
        public void OnGet()
        {
            ProductCart = GetProductsinCart();
            if (ProductCart == null)
            {
                Redirect("/");
            }
            var shipping = _context.Settings.First().Shipping;
            if (shipping != null)
            {
                Order.Shipping = shipping;
            }
        }

        public IActionResult OnPostSubmitOrder()
        {
            ProductCart = GetProductsinCart();
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //-------------------------------------------------------

            //check and find coupon
            if (!string.IsNullOrEmpty(Order.CouponCode))
            {
                var coupon = _context.Coupons.FirstOrDefault(c => c.Code == Order.CouponCode);

                if (coupon != null)
                {
                    Order.CouponCode = coupon.Code;
                    Order.CouponDiscount = coupon.Discount;
                }
                else
                {
                    TempData["message"] = "Coupon not exitst";
                    return Page();
                }
            }


            Order.Shipping = _context.Settings.First().Shipping;
            Order.CreateDate = DateTime.Now;
            Order.SubTotal = ProductCart.Sum(x => x.RowSumPrice);
            Order.Total = (Order.SubTotal + Order.Shipping ?? 0);
            Order.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (Order.CouponDiscount != null)
            {
                Order.Total -= Order.CouponDiscount;
            }

            _context.Orders.Add(Order);
            _context.SaveChanges();

            //-------------------------------------------------------

            List<OrderDetail> orderDetails = new List<OrderDetail>();

            foreach (var item in ProductCart)
            {
                OrderDetail orderDetailItem = new OrderDetail()
                {
                    Count = item.Count,
                    ProductTitle = item.Title,
                    ProductPrice = (decimal)item.Price,
                    OrderId = Order.Id,
                    ProductId = item.Id
                };

                orderDetails.Add(orderDetailItem);
            }

            //-------------------------------------------------------
            _context.OrderDetails.AddRange(orderDetails);
            _context.SaveChanges();

            // Redirect to PayPal
            return Redirect("/Checkout?handler=RedirectToPayPal&OrderId=" + Order.Id);
        }


        public IActionResult OnPostApplyCouponCode(string couponCode)
        {

            ProductCart = GetProductsinCart();

            var coupon = _context.Coupons.FirstOrDefault(c => c.Code == couponCode);

            if (coupon != null)
            {
                Order.CouponCode = coupon.Code;
                Order.CouponDiscount = coupon.Discount;
            }
            else
            {
                TempData["message"] = "Coupon not exitst";
                return Page();
            }

            var shipping = _context.Settings.First().Shipping;
            if (shipping != null)
            {
                Order.Shipping = shipping;
            }

            return Page();
        }


        public ActionResult OnGetRedirectToPayPal(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return RedirectToPage("PaymentReturn");
            }

            var orderDetails = _context.OrderDetails.Where(x => x.OrderId == orderId).ToList();

            var clientId = _configuration.GetValue<string>("PayPal:Key");
            var clientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");
            var apiContext = PaypalConfiguration.GetAPIContext(clientId, clientSecret, mode);

            try
            {
                string baseURI = $"{Request.Scheme}://{Request.Host}/PaymentReturn?";
                var guid = Guid.NewGuid().ToString();

                var payment = new Payment
                {
                    intent = "sale",
                    payer = new Payer { payment_method = "paypal" },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            description = $"Order {order.Id}",
                            invoice_number = Guid.NewGuid().ToString(),
                            amount = new Amount
                            {
                                currency = "USD",
                                total = order.Total?.ToString("F"),
                            },
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        cancel_url = $"{baseURI}&Cancel=true",
                        return_url = $"{baseURI}orderId={order.Id}"
                    }
                };


                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel.ToLower() == "approval_url")?.href;

                _httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);

                return Redirect(approvalUrl);
            }
            catch (Exception e)
            {
                return RedirectToPage("PaymentReturn");
            }
        }

        public List<ProductCartViewModel> GetProductsinCart()
        {
            var cartItems = GetCartItems();

            if (!cartItems.Any())
            {
                return null;
            }

            var cartItemProductIds = cartItems.Select(x => x.ProductId).ToList();
            // Load products into memory
            var products = _context.Products
                .Where(p => cartItemProductIds.Contains(p.Id))
                .ToList();

            // Create the ProductCartViewModel list

            List<ProductCartViewModel> result = new List<ProductCartViewModel>();
            foreach (var item in products)
            {
                var newItem = new ProductCartViewModel
                {
                    Id = item.Id,
                    ImageName = item.ImageName,
                    Price = item.Price - (item.Discount ?? 0),
                    Title = item.Title,
                    Count = cartItems.Single(x => x.ProductId == item.Id).Count,
                    RowSumPrice = (item.Price - (item.Discount ?? 0)) *
                                  cartItems.Single(x => x.ProductId == item.Id).Count,
                };

                result.Add(newItem);
            }

            return result;
        }

        public List<CartViewModel> GetCartItems()
        {
            List<CartViewModel> cartList = new List<CartViewModel>();

            var prevCartItemsString = Request.Cookies["Cart"];

            // If it's not null, it means the cart is not empty, so we need to convert it to a list of view models; 
            // otherwise, we return an empty cart list.
            if (!string.IsNullOrEmpty(prevCartItemsString))
            {
                cartList = JsonConvert.DeserializeObject<List<CartViewModel>>(prevCartItemsString);
            }

            return cartList;
        }

    }
}
