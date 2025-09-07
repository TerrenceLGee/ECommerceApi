using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.Categories.Request;

public class CreateCategoryRequest
{
    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}