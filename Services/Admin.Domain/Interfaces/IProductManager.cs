using Admin.Domain.Models;
using IronBridge.Shared.DTOs;

namespace Admin.Domain.Interfaces;

public interface IProductManager
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int productId);
    Task<ProductDto?> CreateProductAsync(CreateProductDto dto, Guid adminUserId);
    Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto, Guid adminUserId);
    Task<bool> DeleteProductAsync(int productId, Guid adminUserId);
}
