using ECommerce.Api.Dtos.Address.Response;
using ECommerce.Api.Dtos.Auth.Response;
using ECommerce.Api.Dtos.Categories.Response;
using ECommerce.Api.Dtos.Products.Response;
using ECommerce.Api.Dtos.Sales.Response;
using ECommerce.Api.Identity;
using ECommerce.Api.Models;

namespace ECommerce.Api.Mappings;

public static class ToDto
{
    public static CategoryResponse MapCategoryToCategoryResponse(this Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            Products = category.Products
                .Select(p => p.MapProductToProductResponse()).ToList(),
        };
    }

    public static CategoryResponse MapCategoryToCategoryResponseForProductResponse(this Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
        };
    }
    public static ProductResponse MapProductToProductResponse(this Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            StockKeepingUnit = product.StockKeepingUnit,
            Price = product.Price,
            Discount = product.Discount,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            Category = product.Category.MapCategoryToCategoryResponseForProductResponse(),
        };
    }

    public static SaleResponse MapFromSaleToSaleResponse(this Sale sale)
    {
        return new SaleResponse
        {
            Id = sale.Id,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            CustomerId = sale.CustomerId,
            TotalPrice = sale.TotalPrice,
            Status = sale.Status,
            Notes = sale.Notes,
            SaleItems = sale.SaleItems
                .Select(si => si.MapFromSaleProductToSaleProductResponse()).ToList(),
        };
    }

    public static SaleProductResponse MapFromSaleProductToSaleProductResponse(this SaleProduct saleProduct)
    {
        return new SaleProductResponse
        {
            ProductId = saleProduct.ProductId,
            ProductName = saleProduct.Product.Name,
            Quantity = saleProduct.Quantity,
            UnitPrice = saleProduct.UnitPrice,
            DiscountPrice = saleProduct.DiscountPrice,
            GrossPrice = saleProduct.GrossPrice,
            FinalPrice = saleProduct.FinalPrice,
        };
    }

    public static AddressResponse MapAddressToAddressResponse(this Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,
            StreetNumber = address.StreetNumber,
            StreetName = address.StreetName,
            City = address.City,
            State = address.State,
            Country = address.Country,
            ZipCode = address.ZipCode,
            AddressType = address.AddressType
        };
    }

    public static UserResponse MapApplicationUserToUserResponse(this ApplicationUser applicationUser)
    {
        return new UserResponse
        {
            UserId = applicationUser.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            EmailAddress = applicationUser.Email!,
            BirthDate = applicationUser.DateOfBirth,
            Age = GetCorrectAgeForUser(applicationUser.DateOfBirth),
            Addresses = applicationUser.Addresses
                .Select(a => a.MapAddressToAddressResponse()).ToList()
        };
    }


    private static int GetCorrectAgeForUser(DateOnly birthDate)
    {
        return birthDate.DayOfYear > DateTime.Now.DayOfYear
            ? DateTime.Now.AddYears(-1).Year - birthDate.Year
            : DateTime.Now.Year - birthDate.Year;
    }
}