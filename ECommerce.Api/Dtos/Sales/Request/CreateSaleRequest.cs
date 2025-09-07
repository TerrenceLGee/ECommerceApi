using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Sales.Request;

public class CreateSaleRequest
{
    public string? Notes { get; set; }

    [Required] [MinLength(1)] public List<SaleItemRequest> Items { get; set; } = [];
}