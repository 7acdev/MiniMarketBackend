namespace Minimarket.DTO;

public partial class SaleDTO
{
    public int? Id { get; set; }

    public string? DocumentNumber { get; set; }

    public string? PaidType { get; set; }

    public string? TotalText { get; set; }

    public DateTime? RegisterDate { get; set; }

    public virtual ICollection<SaleDetailDTO>? SaleDetails { get; set; }
}