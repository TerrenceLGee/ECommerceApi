using ECommerce.Presentation.Enums;

namespace ECommerce.Presentation.Dtos.Sales.Request;

public class UpdateSaleStatusRequest
{
    public SaleStatus UpdatedStatus { get; set; }
}