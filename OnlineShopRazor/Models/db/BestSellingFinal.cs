using System;
using System.Collections.Generic;

namespace OnlineShopRazor.Models.db;

public partial class BestSellingFinal
{
    public int ProductId { get; set; }

    public string? ImageName { get; set; }

    public decimal? Discount { get; set; }

    public int? Qty { get; set; }

    public string? Title { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public int? TotalCount { get; set; }
}
