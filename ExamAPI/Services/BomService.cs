using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExamAPI.Services
{
    public class BomService : IBomService
    {
        private readonly ExamSQLContext _context;
        private readonly IBomRepository _bomRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<BomService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BomService(IBomRepository bomRepository, IUserRepository userRepository, IProductRepository productRepository
            , IMaterialRepository materialRepository, IOrderRepository orderRepository
            , ILogger<BomService> logger, IHttpContextAccessor httpContextAccessor, ExamSQLContext context)
        {
            _bomRepository = bomRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _materialRepository = materialRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
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
            response.PageCount = pagedList.PageCount;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = $"取得第{page}頁，{pageSize}筆資料";
            _logger.LogTrace("離開GetAllBomsAsync");
            return response;
        }
        public async Task<BomResponse> AddBomAsync(BomRequest bomRequest)
        {
            _logger.LogTrace("進入AddBomAsync");
            BomResponse response = new BomResponse();

            try
            {
                if (bomRequest == null)
                {
                    _logger.LogWarning("新增物料資料為空");
                    response.Success = false;
                    response.Message = "新增物料資料為空";
                    _logger.LogTrace("離開AddBomAsync");
                    return response;
                }
                if (string.IsNullOrEmpty(bomRequest.ProductNo) || string.IsNullOrEmpty(bomRequest.MaterialNo))
                {
                    _logger.LogWarning("必填欄位不能為空");
                    response.Success = false;
                    response.Message = "必填欄位不能為空";
                    _logger.LogTrace("離開AddBomAsync");
                    return response;
                }
                var product = await _productRepository.GetProductByProductNoAsync(bomRequest.ProductNo);
                var materisl = await _materialRepository.GetMaterialByMaterialNoAsync(bomRequest.MaterialNo);
                if (product == null || materisl == null)
                {
                    _logger.LogWarning("無此產品或物料");
                    response.Success = false;
                    response.Message = "無此產品或物料";
                    _logger.LogTrace("離開AddBomAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                var bom = new Bom
                {
                    ProductNo = bomRequest.ProductNo,
                    MaterialNo = bomRequest.MaterialNo,
                    MaterialUseQuantity = bomRequest.MaterialUseQuantity,
                    CreateDate = DateTime.Now,
                    Creator = user.UserName,
                    ModifyDate = DateTime.Now,
                    Modifier = "System"
                };
                await _bomRepository.AddBomAsync(bom);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var b = new BomDto
                    {
                        SerialNo = bom.SerialNo,
                        ProductNo = bom.ProductNo,
                        MaterialNo = bom.MaterialNo,
                        MaterialUseQuantity = bom.MaterialUseQuantity,
                    };
                    _logger.LogInformation("成功新增Bom{SerialNo}", bom.SerialNo);
                    response.Boms = new List<BomDto> { b };
                    response.Success = true;
                    response.Message = "新增Bom成功";
                }
                else
                {
                    _logger.LogWarning("新增Bom失敗");
                    response.Success = false;
                    response.Message = "新增Bom失敗";
                }
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
                if (serialNo == 0 || bomRequest == null)
                {
                    _logger.LogWarning("SerialNo或更新資料為空");
                    response.Success = false;
                    response.Message = "SerialNo或更新資料為空";
                    _logger.LogTrace("離開UpdateBomAsync");
                    return response;
                }
                var existBom = await _bomRepository.GetBomBySerialNoAsync(serialNo);
                if (existBom == null)
                {
                    _logger.LogWarning("Bom{SerialNo}不存在", serialNo);
                    response.Success = false;
                    response.Message = "Bom不存在";
                    _logger.LogTrace("離開UpdateBomAsync");
                    return response;
                }
                var product = await _productRepository.GetProductByProductNoAsync(bomRequest.ProductNo);
                var materisl = await _materialRepository.GetMaterialByMaterialNoAsync(bomRequest.MaterialNo);
                if (product == null || materisl == null)
                {
                    _logger.LogWarning("無此產品或物料");
                    response.Success = false;
                    response.Message = "無此產品或物料";
                    _logger.LogTrace("離開AddBomAsync");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                existBom.ProductNo = string.IsNullOrEmpty(bomRequest.ProductNo)
                    ? existBom.ProductNo : bomRequest.ProductNo;
                existBom.MaterialNo = string.IsNullOrEmpty(bomRequest.MaterialNo)
                    ? existBom.MaterialNo : bomRequest.MaterialNo;
                existBom.MaterialUseQuantity = existBom.MaterialUseQuantity == null
                    ? existBom.MaterialUseQuantity : bomRequest.MaterialUseQuantity;
                existBom.ModifyDate = DateTime.Now;
                existBom.Modifier = user.UserName;
                await _bomRepository.UpdateBomAsync(existBom);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var b = new BomDto
                    {
                        SerialNo = serialNo,
                        ProductNo = existBom.ProductNo,
                        MaterialNo = existBom.MaterialNo,
                        MaterialUseQuantity = existBom.MaterialUseQuantity,
                    };
                    _logger.LogInformation("成功修改Bom{SerialNo}", serialNo);
                    response.Boms = new List<BomDto> { b };
                    response.Success = true;
                    response.Message = "修改Bom成功";
                }
                else
                {
                    _logger.LogWarning("修改Bom失敗");
                    response.Success = false;
                    response.Message = "修改Bom失敗";
                }
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
        // 若Bom關聯的產品，被下過訂單則不可以刪除，但是可以修改
        public async Task<BomResponse> DeleteBomAsync(int serialNo)
        {
            _logger.LogTrace("進入DeleteBomAsync");
            BomResponse response = new BomResponse();

            try
            {
                if(serialNo == 0)
                {
                    _logger.LogWarning("SerialNo為零");
                    response.Success = false;
                    response.Message = "SerialNo為零";
                    _logger.LogTrace("離開DeleteBomAsync");
                    return response;
                }
                var bom = await _bomRepository.GetBomBySerialNoAsync(serialNo);
                if(bom == null)
                {
                    _logger.LogWarning("Bom{SerialNo}不存在", serialNo);
                    response.Success = false;
                    response.Message = "Bom不存在";
                    _logger.LogTrace("離開DeleteBomAsync");
                    return response;
                }
                var detail = _orderRepository.GetAllOrders().SelectMany(od => od.OrderDetails);
                if(detail.Any(x=>x.ProductNo == bom.ProductNo))
                {
                    _logger.LogWarning("Bom關聯的產品，被下過訂單，不可以刪除");
                    response.Success = false;
                    response.Message = "Bom關聯的產品，被下過訂單，不可以刪除";
                    _logger.LogTrace("離開DeleteBomAsync");
                    return response;
                }
                await _bomRepository.CancelBomAsync(bom);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    _logger.LogInformation("成功取消Bom{SerialNo}", serialNo);
                    response.Success = true;
                    response.Message = "取消成功";
                }
                else
                {
                    _logger.LogWarning("取消訂單Bom{SerialNo}", serialNo);
                    response.Success = false;
                    response.Message = "取消失敗";
                }
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
