using IronBridge.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Product.Service.Data;

namespace Product.Service.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _context;

    public ProductService(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products.ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            IsActive = p.IsActive
        });
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive && p.Stock > 0)
            .ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            IsActive = p.IsActive
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return null;

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

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(string category)
    {
        var products = await _context.Products
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync();

        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductName = p.ProductName,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            Category = p.Category,
            ImageUrl = p.ImageUrl,
            CreatedBy = p.CreatedBy,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt,
            IsActive = p.IsActive
        });
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

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

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

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return null;

        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.Category = dto.Category;
        product.ImageUrl = dto.ImageUrl;
        product.IsActive = dto.IsActive;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

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
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return false;

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null || product.Stock < quantity)
            return false;

        product.Stock -= quantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}
