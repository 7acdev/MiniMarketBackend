namespace Minimarket.DTO;

public class ProductDTO
{
    public int? Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }

    public DateTime? RegisterDate { get; set; }
}