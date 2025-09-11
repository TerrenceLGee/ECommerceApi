using System.ComponentModel.DataAnnotations;
using ECommerce.Presentation.Enums;

namespace ECommerce.Presentation.Dtos.Products.Request;

public class UpdateProductRequest
{
    [Required] [MaxLength(200)] public string Name { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Description { get; set; }
    public string? StockKeepingUnit { get; set; }
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    [Required]
    public DiscountStatus Discount { get; set; }
    [Required]
    public int CategoryId { get; set; }
    [Required]
    public bool IsActive { get; set; }
}