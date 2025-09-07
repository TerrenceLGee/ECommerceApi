using ECommerce.Api.Models.Enums;

namespace ECommerce.Api.Models;

public class Product : BaseEntity
{
    public string? StockKeepingUnit { get; set; }
    public decimal Price { get; set; }
    public DiscountStatus Discount { get; set; } = DiscountStatus.None;
    public int StockQuantity { get; set; } = 0;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<SaleProduct> SaleItems { get; set; } = [];
}