using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Sales.Request;

public class CreateSaleRequest
{
    [Required]
    public string StreetNumber { get; set; } = string.Empty;
    [Required]
    public string StreetName { get; set; } = string.Empty;
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string Country { get; set; } = string.Empty;
    [Required]
    public string ZipCode { get; set; } = string.Empty;
    [Required]
    public string? Notes { get; set; }

    [Required] [MinLength(1)] public List<SaleItemRequest> Items { get; set; } = [];
}