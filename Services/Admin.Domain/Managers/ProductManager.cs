using Admin.Domain.Interfaces;
using Admin.Repository.Interfaces;
using IronBridge.Shared.DTOs;
using IronBridge.Shared.Enums;
using IronBridge.Shared.Interfaces;
using Mapster;

namespace Admin.Domain.Managers;

public class ProductManager : IProductManager
{
    private readonly IProductRepository _productRepository;
    private readonly IUserHttpClient _userClient;
    private readonly ICacheService _cacheService;

    public ProductManager(IProductRepository productRepository, IUserHttpClient userClient, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _userClient = userClient;
        _cacheService = cacheService;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var cacheKey = "products_all";
        var cachedProducts = await _cacheService.GetAsync<IEnumerable<ProductDto>>(cacheKey);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetAllAsync();
        var productDtos = products.Adapt<IEnumerable<ProductDto>>();

        await _cacheService.SetAsync(cacheKey, productDtos, TimeSpan.FromMinutes(5));
        return productDtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        var cacheKey = $"product_{productId}";
        var cachedProduct = await _cacheService.GetAsync<ProductDto>(cacheKey);

        if (cachedProduct != null)
            return cachedProduct;

        var product = await _productRepository.GetByIdAsync(productId);
        var productDto = product?.Adapt<ProductDto>();

        if (productDto != null)
            await _cacheService.SetAsync(cacheKey, productDto, TimeSpan.FromMinutes(5));

        return productDto;
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
        var createdProductDto = createdProduct.Adapt<ProductDto>();

        await _cacheService.RemoveAsync("products_all");
        await _cacheService.SetAsync($"product_{createdProduct.Id}", createdProductDto, TimeSpan.FromMinutes(5));

        return createdProductDto;
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
        var updatedProductDto = updatedProduct?.Adapt<ProductDto>();

        await _cacheService.RemoveAsync($"product_{productId}");
        await _cacheService.RemoveAsync("products_all");

        return updatedProductDto;
    }

    public async Task<bool> DeleteProductAsync(int productId, Guid adminUserId)
    {
        // Verify user is admin
        var user = await _userClient.GetUserByIdAsync(adminUserId);

        if (user == null || user.Role != UserRole.Admin)
            return false;

        var result = await _productRepository.DeleteAsync(productId);

        if (result)
        {
            await _cacheService.RemoveAsync($"product_{productId}");
            await _cacheService.RemoveAsync("products_all");
        }

        return result;
    }
}
