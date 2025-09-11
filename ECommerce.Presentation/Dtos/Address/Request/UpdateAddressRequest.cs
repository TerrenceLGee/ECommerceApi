using System.ComponentModel.DataAnnotations;

namespace ECommerce.Presentation.Dtos.Address.Request;

public class UpdateAddressRequest
{
    [Required] public string StreetNumber { get; set; } = string.Empty;
    [Required] public string StreetName { get; set; } = string.Empty;
    [Required] public string City { get; set; } = string.Empty;
    [Required] public string State { get; set; } = string.Empty;
    [Required] public string Country { get; set; } = string.Empty;
    [Required] public string ZipCode { get; set; } = string.Empty;
    [Required] public string AddressType { get; set; } = string.Empty;
}