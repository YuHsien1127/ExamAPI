using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IProductRepository
    {
        public IQueryable<Product> GetAllProducts();
        public Task<Product> GetProductByProductNoAsync(string ProductNo);
        public Task AddProductAsync(Product product);
        public Task UpdateProductAsync(Product product);
        public Task DeleteProductAsync(Product product);
    }
}
