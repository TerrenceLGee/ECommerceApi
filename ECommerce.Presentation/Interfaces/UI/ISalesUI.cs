namespace ECommerce.Presentation.Interfaces.UI;

public interface ISalesUI
{
    Task<bool> HandleViewAllSalesAsync();
    Task<bool> HandleViewAllSalesForUserAsync();
    Task HandleViewSaleByIdAsync();
    Task HandleViewUserSaleByIdAsync();
    Task HandleCreateSaleAsync();
    Task HandleUpdateSaleStatusAsync();
    Task HandleRefundSaleAsync();
    Task HandleAdminCancelSaleAsync();
    Task HandleUserCancelSaleAsync();
}