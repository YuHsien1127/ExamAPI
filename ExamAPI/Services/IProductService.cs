using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IProductService
    {
        public ProductResponse GetAllProducts(int page, int pageSize);
        public Task<ProductResponse> AddProductAsync(ProductRequest productRequest);
        public Task<ProductResponse> UpdateProductAsync(string productNo, ProductRequest productRequest);
        public Task<ProductResponse> DeleteProductAsync(string productNo);
    }
}
