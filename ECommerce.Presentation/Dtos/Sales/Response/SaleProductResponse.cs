namespace ECommerce.Presentation.Dtos.Sales.Response;

public class SaleProductResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public decimal GrossPrice { get; set; }
    public decimal FinalPrice { get; set; }
}