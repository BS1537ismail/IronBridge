using IronBridge.Shared.DTOs;

namespace Admin.Service.Services;

public interface IProductHttpClient
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int productId);
    Task<ProductDto?> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int productId);
}
