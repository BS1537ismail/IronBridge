using IronBridge.Shared.DTOs;
using IronBridge.Shared.Interfaces;
using Mapster;
using Product.Domain.Interfaces;
using Product.Repository.Interfaces;

namespace Product.Domain.Managers;

public class ProductManager : IProductManager
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public ProductManager(IProductRepository productRepository, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<IList<Models.Product>> GetAllProductsAsync()
    {
        var cacheKey = "products_all";
        var cachedProducts = await _cacheService.GetAsync<IList<Models.Product>>(cacheKey);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetAllAsync();
        var productList = products.Adapt<IList<Models.Product>>();

        await _cacheService.SetAsync(cacheKey, productList, TimeSpan.FromMinutes(5));
        return productList;
    }

    public async Task<IList<Models.Product>> GetActiveProductsAsync()
    {
        var cacheKey = "products_active";
        var cachedProducts = await _cacheService.GetAsync<IList<Models.Product>>(cacheKey);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetActiveProductsAsync();
        var productList = products.Adapt<IList<Models.Product>>();

        await _cacheService.SetAsync(cacheKey, productList, TimeSpan.FromMinutes(5));
        return productList;
    }

    public async Task<Models.Product?> GetProductByIdAsync(int id)
    {
        var cacheKey = $"product_{id}";
        var cachedProduct = await _cacheService.GetAsync<Models.Product>(cacheKey);

        if (cachedProduct != null)
            return cachedProduct;

        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        var productModel = product.Adapt<Models.Product>();
        await _cacheService.SetAsync(cacheKey, productModel, TimeSpan.FromMinutes(5));

        return productModel;
    }

    public async Task<IList<Models.Product>> GetProductsByCategoryAsync(string category)
    {
        var cacheKey = $"products_category_{category}";
        var cachedProducts = await _cacheService.GetAsync<IList<Models.Product>>(cacheKey);

        if (cachedProducts != null)
            return cachedProducts;

        var products = await _productRepository.GetProductsByCategoryAsync(category);
        var productList = products.Adapt<IList<Models.Product>>();

        await _cacheService.SetAsync(cacheKey, productList, TimeSpan.FromMinutes(5));
        return productList;
    }

    public async Task<ProductDto?> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Models.Product
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

        var savedProduct = await _productRepository.AddAsync(product.Adapt<Repository.Models.Product>());

        var productDto = new ProductDto
        {
            Id = savedProduct.Id,
            ProductName = savedProduct.ProductName,
            Description = savedProduct.Description,
            Price = savedProduct.Price,
            Stock = savedProduct.Stock,
            Category = savedProduct.Category,
            ImageUrl = savedProduct.ImageUrl,
            CreatedBy = savedProduct.CreatedBy,
            CreatedAt = savedProduct.CreatedAt,
            UpdatedAt = savedProduct.UpdatedAt,
            IsActive = savedProduct.IsActive
        };

        await _cacheService.RemoveAsync("products_all");
        await _cacheService.RemoveAsync("products_active");
        await _cacheService.RemoveAsync($"products_category_{savedProduct.Category}");

        return productDto;
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        var oldCategory = product.Category;

        product.ProductName = dto.ProductName != "string" ? dto.ProductName : product.ProductName;
        product.Description = dto.Description != "string" ? dto.Description : product.Description;
        product.Price = dto.Price != 0 ? dto.Price : product.Price;
        product.Stock = dto.Stock != 0 ? dto.Stock : product.Stock;
        product.Category = dto.Category != "string" ? dto.Category : product.Category;
        product.ImageUrl = dto.ImageUrl != "string" ? dto.ImageUrl : product.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        var productDto = new ProductDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            Category = product.Category,
            ImageUrl = product.ImageUrl,
            CreatedBy = product.CreatedBy,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            IsActive = product.IsActive
        };

        await _cacheService.RemoveAsync($"product_{id}");
        await _cacheService.RemoveAsync("products_all");
        await _cacheService.RemoveAsync("products_active");
        await _cacheService.RemoveAsync($"products_category_{oldCategory}");
        await _cacheService.RemoveAsync($"products_category_{product.Category}");

        return productDto;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        await _cacheService.RemoveAsync($"product_{id}");
        await _cacheService.RemoveAsync("products_all");
        await _cacheService.RemoveAsync("products_active");
        await _cacheService.RemoveAsync($"products_category_{product.Category}");

        return true;
    }

    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null || product.Stock < quantity)
            return false;

        product.Stock -= quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        await _cacheService.RemoveAsync($"product_{id}");
        await _cacheService.RemoveAsync("products_all");
        await _cacheService.RemoveAsync("products_active");
        await _cacheService.RemoveAsync($"products_category_{product.Category}");

        return true;
    }
}
