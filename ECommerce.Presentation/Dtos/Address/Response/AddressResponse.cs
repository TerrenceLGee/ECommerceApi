namespace ECommerce.Presentation.Dtos.Address.Response;

public class AddressResponse
{
    public int Id { get; set; }
    public string StreetNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string AddressType { get; set; } = string.Empty;
    public string FullAddress => $"{StreetNumber} {StreetName} {City}, {State}, {ZipCode} {Country}";
}