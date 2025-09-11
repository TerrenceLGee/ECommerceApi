using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Dtos.Categories.Request;

public class UpdateCategoryRequest
{
    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}