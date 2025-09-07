namespace ECommerce.Api.Models;

public class SaleProduct
{
    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPrice { get; set; }
    public decimal GrossPrice => Quantity * UnitPrice;
    public decimal FinalPrice => Quantity * DiscountPrice;

}