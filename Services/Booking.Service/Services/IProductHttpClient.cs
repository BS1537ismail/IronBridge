using IronBridge.Shared.DTOs;

namespace Booking.Service.Services;

public interface IProductHttpClient
{
    Task<ProductDto?> GetProductByIdAsync(int productId);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
