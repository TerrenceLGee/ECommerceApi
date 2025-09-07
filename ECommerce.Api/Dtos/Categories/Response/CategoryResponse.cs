using ECommerce.Api.Dtos.Products.Response;
using ECommerce.Api.Models;

namespace ECommerce.Api.Dtos.Categories.Response;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<ProductResponse> Products { get; set; } = [];
}