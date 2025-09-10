using ECommerce.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Api.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}