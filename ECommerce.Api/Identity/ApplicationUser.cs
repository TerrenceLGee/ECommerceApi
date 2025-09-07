using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Api.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleInitial { get; set; }
    public string LastName { get; set; } = string.Empty;
    public Address ShippingAddress { get; set; } = null!;
    public Address? BillingAddress { get; set; }
}