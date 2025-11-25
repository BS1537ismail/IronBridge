using Mapster;
using Microsoft.EntityFrameworkCore;
using Product.Repository.Interfaces;
using Product.Repository.Models;

namespace Product.Repository.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Models.Product>> GetAllAsync()
    {
        var products = await _context.Products.ToListAsync();
        return products;
    }

    public async Task<IList<Models.Product>> GetActiveProductsAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive && p.Stock > 0)
            .ToListAsync();
        return products;
    }

    public async Task<Models.Product?> GetByIdAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product;
    }

    public async Task<IList<Models.Product>> GetProductsByCategoryAsync(string category)
    {
        var products = await _context.Products
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync();
        return products;
    }

    public async Task<Models.Product> AddAsync(Models.Product product)
    {
        var entity = product.Adapt<Models.Product>();
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Models.Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}
