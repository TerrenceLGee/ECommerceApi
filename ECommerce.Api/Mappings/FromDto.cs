using ECommerce.Api.Dtos.Address.Request;
using ECommerce.Api.Dtos.Categories.Request;
using ECommerce.Api.Dtos.Products.Request;
using ECommerce.Api.Dtos.Sales.Request;
using ECommerce.Api.Models;

namespace ECommerce.Api.Mappings;

public static class FromDto
{
    public static Category MapFromCreateCategoryRequestToCategory(this CreateCategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Description = request.Description
        };
    }
    
    
    public static void MapFromUpdateCategoryRequestToCategory(this UpdateCategoryRequest request, Category category)
    {
        category.Name = request.Name;
        category.Description = request.Description;
    }
    
    public static Product MapFromCreateProductRequestToProduct(this CreateProductRequest request)
    {
        return new Product
        {
            Name = request.Name,
            Description = request.Description,
            StockKeepingUnit = request.StockKeepingUnit,
            Price = request.Price,
            Discount = request.Discount,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId
        };
    }

    public static void UpdateProductFromUpdateProductRequest(this UpdateProductRequest request, Product product)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.StockKeepingUnit = request.StockKeepingUnit;
        product.Discount = request.Discount;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
    }

    public static Product MapFromSaleProductToProduct(this SaleProduct saleProduct)
    {
        return new Product
        {
            Name = saleProduct.Product.Name,
            Description = saleProduct.Product.Description,
            CreatedAt = saleProduct.Product.CreatedAt,
            StockKeepingUnit = saleProduct.Product.StockKeepingUnit,
            Price = saleProduct.Product.Price,
            Discount = saleProduct.Product.Discount,
            StockQuantity = saleProduct.Product.StockQuantity,
            CategoryId = saleProduct.Product.CategoryId,
        };
    }

    public static Sale MapFromCreateSaleRequestToSale(this CreateSaleRequest request, string customerId)
    {
        return new Sale
        {
            CustomerId = customerId,
            Notes = request.Notes,
            StreetNumber = request.StreetNumber,
            StreetName = request.StreetName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
        };
    }

    public static Address MapFromCreateAddressRequestToAddress(this CreateAddressRequest request)
    {
        return new Address
        {
            StreetNumber = request.StreetNumber,
            StreetName = request.StreetName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            Description = request.Description,
        };
    }

    public static void MapFromUpdateAddressRequestToAddress(this UpdateAddressRequest request, Address address)
    {
        address.StreetNumber = request.StreetNumber;
        address.StreetName = request.StreetName;
        address.City = request.City;
        address.State = request.State;
        address.Country = request.Country;
        address.ZipCode = request.ZipCode;
        address.Description = request.Description;
    }
}