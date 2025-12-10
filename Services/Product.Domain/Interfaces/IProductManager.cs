using IronBridge.Shared.DTOs;

namespace Product.Domain.Interfaces;

public interface IProductManager
{
    Task<IList<Models.Product>> GetAllProductsAsync();
    Task<IList<Models.Product>> GetActiveProductsAsync();
    Task<Models.Product?> GetProductByIdAsync(int id);
    Task<IList<Models.Product>> GetProductsByCategoryAsync(string category);
    //Task<ProductDto?> CreateProductAsync(CreateProductDto dto);
    //Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto);
    //Task<bool> DeleteProductAsync(int id);
    //Task<bool> UpdateStockAsync(int id, int quantity);
}
