using Admin.Domain.Interfaces;
using Admin.Repository.Interfaces;
using IronBridge.Shared.DTOs;
using IronBridge.Shared.Enums;
using Mapster;

namespace Admin.Domain.Managers;

public class ProductManager : IProductManager
{
    private readonly IProductRepository _productRepository;
    private readonly IUserHttpClient _userClient;

    public ProductManager(IProductRepository productRepository, IUserHttpClient userClient)
    {
        _productRepository = productRepository;
        _userClient = userClient;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Adapt<IEnumerable<ProductDto>>();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product?.Adapt<ProductDto>();
    }

    public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto, Guid adminUserId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminUserId);

        if (user == null || user.Role != UserRole.Admin)
            return null;

        var product = new Repository.Models.Product
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Category = dto.Category,
            ImageUrl = dto.ImageUrl,
            CreatedBy = dto.CreatedBy,
            IsActive = true
        };

        var createdProduct = await _productRepository.CreateAsync(product);
        return createdProduct.Adapt<ProductDto>();
    }

    public async Task<ProductDto?> UpdateProductAsync(int productId, UpdateProductDto dto, Guid adminUserId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminUserId);

        if (user == null || user.Role != UserRole.Admin)
            return null;

        var product = await _productRepository.GetByIdAsync(productId);

        if (product == null)
            return null;

        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Category = dto.Category;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;

        var updatedProduct = await _productRepository.UpdateAsync(product);
        return updatedProduct?.Adapt<ProductDto>();
    }

    public async Task<bool> DeleteProductAsync(int productId, Guid adminUserId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminUserId);

        if (user == null || user.Role != UserRole.Admin)
            return false;

        return await _productRepository.DeleteAsync(productId);
    }
}
