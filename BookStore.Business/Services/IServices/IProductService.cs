using BookStore.Models;

namespace BookStore.Business.Services.IServices
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false);
        Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory = false);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
