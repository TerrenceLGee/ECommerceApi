using ECommerce.Presentation.Dtos.Products.Response;

namespace ECommerce.Presentation.Dtos.Categories.Response;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<ProductResponse> Products { get; set; } = [];
}