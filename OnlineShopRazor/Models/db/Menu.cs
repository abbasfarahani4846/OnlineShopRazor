using System;
using System.Collections.Generic;

namespace OnlineShopRazor.Models.db;

public partial class Menu
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Link { get; set; }

    public string Type { get; set; } = null!;
}
