using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ExamSQLContext _context;
        public ProductRepository(ExamSQLContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products;
        }

        public async Task<Product> GetProductByProductNoAsync(string ProductNo)
        {
            return await _context.Products.FindAsync(ProductNo);
        }

        public async Task AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await Task.CompletedTask;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await Task.CompletedTask;
        }
    }
}
