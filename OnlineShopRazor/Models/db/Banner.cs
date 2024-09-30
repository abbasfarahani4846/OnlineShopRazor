using System;
using System.Collections.Generic;

namespace OnlineShopRazor.Models.db;

public partial class Banner
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? ImageName { get; set; }

    public int? Priority { get; set; }

    public string? Link { get; set; }

    public string? Position { get; set; }
}
