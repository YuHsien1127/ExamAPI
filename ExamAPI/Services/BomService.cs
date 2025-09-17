using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ExamAPI.Services
{
    public class BomService : IBomService
    {
        private readonly IBomRepository _bomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<BomService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BomService(IBomRepository bomRepository, IUserRepository userRepository, IProductRepository productRepository, IMaterialRepository materialRepository, IOrderRepository orderRepository, ILogger<BomService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _bomRepository = bomRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _materialRepository = materialRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public BomResponse GetAllBoms(int page, int pageSize)
        {
            _logger.LogTrace("進入GetAllBomsAsync");
            BomResponse response = new BomResponse();
            var bom = _bomRepository.GetAllBoms();
            var b = bom.Select(x => new BomDto
            {
                SerialNo = x.SerialNo,
                ProductNo = x.ProductNo,
                MaterialNo = x.MaterialNo,
                MaterialUseQuantity = x.MaterialUseQuantity
            });
            _logger.LogDebug("取得訂單數量：{b.Count()}", b.Count());
            var pagedList = b.ToPagedList(page, pageSize);
            response.Boms = pagedList.ToList();
            response.PageCount = pagedList.Count;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = "查詢成功";
            _logger.LogTrace("離開GetAllBomsAsync");
            return response;
        }
        public async Task<BomResponse> AddBomAsync(BomRequest bomRequest)
        {
            _logger.LogTrace("進入AddBomAsync");
            BomResponse response = new BomResponse();

            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增發生錯誤");
                response.Success = false;
                response.Message = "新增發生錯誤";
            }
            _logger.LogTrace("離開AddBomAsync");
            return response;
        }
        public async Task<BomResponse> UpdateBomAsync(int serialNo, BomRequest bomRequest)
        {
            _logger.LogTrace("進入UpdateBomAsync");
            BomResponse response = new BomResponse();

            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改發生錯誤");
                response.Success = false;
                response.Message = "修改發生錯誤";
            }
            _logger.LogTrace("離開UpdateBomAsync");
            return response;
        }
        public async Task<BomResponse> DeleteBomAsync(int serialNo)
        {
            _logger.LogTrace("進入DeleteBomAsync");
            BomResponse response = new BomResponse();

            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除發生錯誤");
                response.Success = false;
                response.Message = "刪除發生錯誤";
            }
            _logger.LogTrace("離開DeleteBomAsync");
            return response;
        }
    }
}
