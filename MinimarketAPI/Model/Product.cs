using System;
using System.Collections.Generic;

namespace Minimarket.Model;

public partial class Product
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public bool? IsActive { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? RegisterDate { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; } = new List<SaleDetail>();
}
