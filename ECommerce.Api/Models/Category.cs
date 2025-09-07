namespace ECommerce.Api.Models;

public class Category : BaseEntity
{
    public ICollection<Product> Products { get; set; } = [];
}