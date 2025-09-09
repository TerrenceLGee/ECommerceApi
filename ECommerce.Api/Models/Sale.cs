using ECommerce.Api.Models.Enums;

namespace ECommerce.Api.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public string StreetNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public ICollection<SaleProduct> SaleItems { get; set; } = [];
}