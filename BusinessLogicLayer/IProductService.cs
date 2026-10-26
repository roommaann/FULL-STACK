using Models;

namespace BusinessLogicLayer;

public interface IProductService
{
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}
