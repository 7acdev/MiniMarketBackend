namespace Minimarket.DTO;

public class SaleDetailDTO
{
    public int? IdProduct { get; set; }

    public string? IdProductDescription { get; set; }

    public int? Amount { get; set; }

    public string? PriceText { get; set; }

    public string? TotalText { get; set; }
}