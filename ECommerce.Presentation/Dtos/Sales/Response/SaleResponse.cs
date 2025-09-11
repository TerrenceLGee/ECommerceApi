using ECommerce.Presentation.Enums;

namespace ECommerce.Presentation.Dtos.Sales.Response;

public class SaleResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public SaleStatus Status { get; set; }
    public string? Notes { get; set; }
    public List<SaleProductResponse> SaleItems { get; set; } = [];
}