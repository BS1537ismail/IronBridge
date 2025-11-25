namespace Product.Repository.Interfaces;

public interface IProductRepository
{
    Task<IList<Models.Product>> GetAllAsync();
    Task<IList<Models.Product>> GetActiveProductsAsync();
    Task<Models.Product?> GetByIdAsync(int id);
    Task<IList<Models.Product>> GetProductsByCategoryAsync(string category);
    Task<Models.Product> AddAsync(Models.Product product);
    Task UpdateAsync(Models.Product product);
}
