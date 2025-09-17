using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExamAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "admin, staff")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        /// <summary>
        /// 查詢產品全部資料
        /// </summary>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">一頁幾筆</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ProductResponse> GetAllProductAsync(int page = 1, int pageSize = 10)
        {
            return await _productService.GetAllProductsAsync(page, pageSize);
        }

        /// <summary>
        /// 新增產品
        /// </summary>
        /// <param name="productRequest">產品資料</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ProductResponse> AddProductAsync([FromBody] ProductRequest productRequest)
        {
            return await _productService.AddProductAsync(productRequest);
        }

        /// <summary>
        /// 修改產品
        /// </summary>
        /// <param name="productNo">產品編號</param>
        /// <param name="productRequest">產品資料</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ProductResponse> UpdateProductAsync(string productNo, [FromBody] ProductRequest productRequest)
        {
            return await _productService.UpdateProductAsync(productNo, productRequest);
        }

        /// <summary>
        /// 刪除產品
        /// </summary>
        /// <param name="productNo">產品編號</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<ProductResponse> DeleteProductAsync(string productNo)
        {
            return await _productService.DeleteProductAsync(productNo);
        }
    }
}
