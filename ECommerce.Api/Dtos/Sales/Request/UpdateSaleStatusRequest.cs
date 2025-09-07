using ECommerce.Api.Models.Enums;

namespace ECommerce.Api.Dtos.Sales.Request;

public class UpdateSaleStatusRequest
{
    public SaleStatus UpdatedStatus { get; set; }
}