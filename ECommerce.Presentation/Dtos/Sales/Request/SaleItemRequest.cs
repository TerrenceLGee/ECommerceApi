using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Dtos.Sales.Request;

public class SaleItemRequest
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }
}