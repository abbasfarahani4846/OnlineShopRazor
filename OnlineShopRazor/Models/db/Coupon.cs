using System;
using System.Collections.Generic;

namespace OnlineShopRazor.Models.db;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal Discount { get; set; }
}
