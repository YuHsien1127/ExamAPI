using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList.Extensions;

namespace ExamAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ExamSQLContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ProductService> _logger;
        public ProductService(ExamSQLContext context, IProductRepository productRepository, ILogger<ProductService> logger, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _context = context;
            _productRepository = productRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        public ProductResponse GetAllProducts(int page, int pageSize)
        {
            _logger.LogTrace("進入GetAllProductsAsync");
            ProductResponse response = new ProductResponse();

            var products = _productRepository.GetAllProducts();
            var p = products.Select(x => new ProductDto
            {
                ProductNo = x.ProductNo,
                ProductName = x.ProductName,
                ProductPrice = x.ProductPrice
            });
            _logger.LogDebug("取得Product數量：{p.Count()}", p.Count());
            var pagedList = p.ToPagedList(page, pageSize);
            response.Products = pagedList.ToList();
            response.PageCount = pagedList.Count;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = "查詢成功";
            _logger.LogTrace("離開GetAllProductsAsync");
            return response;
        }
        public async Task<ProductResponse> AddProductAsync(ProductRequest productRequest)
        {
            _logger.LogTrace("進入AddProduct");
            ProductResponse response = new ProductResponse();

            try
            {
                if (productRequest == null)
                {
                    _logger.LogWarning("新增產品資料為空");
                    response.Success = false;
                    response.Message = "新增產品資料為空";
                    _logger.LogTrace("離開AddProductAsync");
                    return response;
                }
                if(string.IsNullOrEmpty(productRequest.ProductName))
                {
                    _logger.LogWarning("必填欄位不能為空");
                    response.Success = false;
                    response.Message = "必填欄位不能為空";
                    _logger.LogTrace("離開AddProductAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                // 倒序，找到最後一筆資料
                var lastProduct = await _context.Products.OrderByDescending(p => p.ProductNo).FirstOrDefaultAsync();
                int number = 1;
                if (lastProduct != null)
                {
                    // 取出流水號部分
                    string lastProductNo = lastProduct.ProductNo.Substring(2);
                    number = int.Parse(lastProductNo) + 1;
                }
                var product = new Product
                {
                    ProductNo = $"PT{number:D8}",
                    ProductName = productRequest.ProductName,
                    ProductPrice = productRequest.ProductPrice,
                    CreateDate = DateTime.Now,
                    Creator = user.UserName,
                    ModifyDate = DateTime.Now,
                    Modifier = "System"
                };

                await _productRepository.AddProductAsync(product);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var p = new ProductDto
                    {
                        ProductNo = product.ProductNo,
                        ProductName = product.ProductName,
                        ProductPrice = product.ProductPrice
                    };
                    _logger.LogInformation("成功新增產品{ProductNo}", product.ProductNo);
                    response.Products = new List<ProductDto> { p };
                    response.Success = true;
                    response.Message = "新增產品成功";
                }
                else
                {
                    _logger.LogWarning("新增產品失敗");
                    response.Success = false;
                    response.Message = "新增產品失敗";
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "新增發生錯誤");
                response.Success = false;
                response.Message = "新增發生錯誤";
            }
            _logger.LogTrace("離開AddProductAsync");
            return response;
        }
        public async Task<ProductResponse> UpdateProductAsync(string productNo, ProductRequest productRequest)
        {
            _logger.LogTrace("進入UpdateProductAsync");
            ProductResponse response = new ProductResponse();

            try
            {
                if (string.IsNullOrEmpty(productNo) || productRequest == null)
                {
                    _logger.LogWarning("ProductNo或更新資料為空");
                    response.Success = false;
                    response.Message = "ProductNo或更新資料為空";
                    _logger.LogTrace("離開UpdateProductAsync");
                    return response;
                }
                // 檢查產品是否存在
                var existingProduct = await _productRepository.GetProductByProductNoAsync(productNo);
                if (existingProduct == null)
                {
                    _logger.LogWarning("產品{ProductNo}不存在", productNo);
                    response.Success = false;
                    response.Message = "產品不存在";
                    _logger.LogTrace("離開UpdateProductAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                // 更新產品資訊
                existingProduct.ProductName = string.IsNullOrEmpty(productRequest.ProductName)
                    ? existingProduct.ProductName : productRequest.ProductName;
                existingProduct.ProductPrice = productRequest.ProductPrice == null
                    ? existingProduct.ProductPrice : productRequest.ProductPrice;
                existingProduct.ModifyDate = DateTime.Now;
                existingProduct.Modifier = user.UserName;
                await _productRepository.UpdateProductAsync(existingProduct);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var p = new ProductDto
                    {
                        ProductNo = existingProduct.ProductNo,
                        ProductName = existingProduct.ProductName,
                        ProductPrice = existingProduct.ProductPrice
                    };
                    _logger.LogInformation("成功修改產品{ProductNo}", existingProduct.ProductNo);
                    response.Products = new List<ProductDto> { p };
                    response.Success = true;
                    response.Message = "修改產品成功";
                }
                else
                {
                    _logger.LogWarning("修改產品失敗");
                    response.Success = false;
                    response.Message = "修改產品失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改發生錯誤");
                response.Success = false;
                response.Message = "修改發生錯誤";
            }
            _logger.LogTrace("離開UpdateProductAsync");
            return response;
        }
        // 產品曾經被下過訂單則不可以刪除
        public async Task<ProductResponse> DeleteProductAsync(string productNo)
        {
            _logger.LogTrace("進入DeleteProduct");
            ProductResponse response = new ProductResponse();

            try
            {
                if(string.IsNullOrEmpty(productNo))
                {
                    _logger.LogWarning("產品編號為空");
                    response.Success = false;
                    response.Message = "產品編號為空";
                    _logger.LogTrace("離開DeleteProductAsync");
                    return response;
                }
                var product = await _productRepository.GetProductByProductNoAsync(productNo);
                if(product == null)
                {
                    _logger.LogWarning("產品{ProductNo}不存在", productNo);
                    response.Success = false;
                    response.Message = "產品不存在";
                    _logger.LogTrace("離開UpdateProductAsync");
                    return response;
                }
                var orders = _orderRepository.GetAllOrders();
                var query = orders.Include(od => od.OrderDetails);
                bool checkProductNo = query.Any(x => x.OrderDetails.Any(y => y.ProductNo == productNo));
                if(checkProductNo)
                {
                    _logger.LogWarning("產品編號{ProductNo}已經被使用於訂單，不能刪除", productNo);
                    response.Success = false;
                    response.Message = "產品已經被使用於訂單，不能刪除";
                    _logger.LogTrace("離開DeleteProductAsync");
                    return response;
                }
                await _productRepository.DeleteProductAsync(product);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    _logger.LogInformation("成功刪除產品{ProductNo}", productNo);
                    response.Success = true;
                    response.Message = "刪除成功";
                }
                else
                {
                    _logger.LogWarning("刪除產品失敗{ProductNo}", productNo);
                    response.Success = false;
                    response.Message = "刪除失敗";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除發生錯誤");
                response.Success = false;
                response.Message = "刪除發生錯誤";
            }
            _logger.LogTrace("離開DeleteProduct");
            return response;
        }
    }
}
