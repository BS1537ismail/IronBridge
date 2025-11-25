using IronBridge.Shared.DTOs;
using Mapster;
using Product.Domain.Interfaces;
using Product.Repository.Interfaces;

namespace Product.Domain.Managers;

public class ProductManager : IProductManager
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IList<Models.Product>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Adapt<IList<Models.Product>>();
    }

    public async Task<IList<Models.Product>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();

        return products.Adapt<IList<Models.Product>>();
    }

    public async Task<Models.Product?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        return product.Adapt<Models.Product>();
    }

    public async Task<IList<Models.Product>> GetProductsByCategoryAsync(string category)
    {
        var products = await _productRepository.GetProductsByCategoryAsync(category);

        return products.Adapt<IList<Models.Product>>();
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

        return new ProductDto
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
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return null;

        product.ProductName = dto.ProductName != "string" ? dto.ProductName : product.ProductName;
        product.Description = dto.Description != "string" ? dto.Description : product.Description;
        product.Price = dto.Price != 0 ? dto.Price : product.Price;
        product.Stock = dto.Stock != 0 ? dto.Stock : product.Stock;
        product.Category = dto.Category != "string" ? dto.Category : product.Category;
        product.ImageUrl = dto.ImageUrl != "string" ? dto.ImageUrl : product.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        return new ProductDto
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
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
            return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);
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
        return true;
    }
}
