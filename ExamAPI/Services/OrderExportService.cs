using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExamAPI.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly ExamSQLContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderExportService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMaterialRepository _materialRepository;
        private readonly IBomRepository _bomRepository;
        public OrderExportService(ExamSQLContext context, IConfiguration configuration
            , ILogger<OrderExportService> logger, IOrderRepository orderRepository, IProductRepository productRepository
            , IMaterialRepository materialRepository, IBomRepository bomRepository)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _materialRepository = materialRepository;
            _bomRepository = bomRepository;
        }
        public async Task<BaseResponse> Production_ExportOrdersAsync()
        {
            _logger.LogTrace("進入ExportOrders");
            BaseResponse response = new BaseResponse();
            try
            {
                var baseDir = _configuration["ReportExport:BaseDir"] ?? "C:\\Users\\kiko lee\\Downloads\\Order\\Out";
                Directory.CreateDirectory(baseDir);
                var filePath = Path.Combine(baseDir, $"Production_Order_{DateTime.Now:yyyyMMdd}.json");
                _logger.LogInformation("開始匯出訂單");

                // 查詢狀態為「成立」的訂單
                var orders = _orderRepository.GetAllOrders().Where(x => x.Status == "成立").ToList();
                if (!orders.Any())
                {
                    _logger.LogInformation("沒有需要匯出的訂單");
                    response.Success = true;
                    response.Message = "沒有需要匯出的訂單";
                    _logger.LogTrace("離開ExportOrders");
                    return response;
                }
                // 準備匯出資料
                var exportData = new
                {
                    exportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    orders = orders.Select(o => new
                    {
                        orderNo = o.OrderNo,
                        orderSubject = o.OrderSubject,
                        orderApplicant = o.OrderApplicant,
                        status = o.Status,
                        createDate = o.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        details = o.OrderDetails.Select(d => new
                        {
                            serialNo = d.SerialNo,
                            productNo = d.ProductNo,
                            quantity = d.Quantity
                        }).ToList()
                    }).ToList()
                };
                // 序列化為 JSON
                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // 確保目錄存在
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

                // 寫入檔案
                await File.WriteAllTextAsync(filePath, json);

                foreach (var order in orders)
                {
                    var details = order.OrderDetails.ToList();

                    // 扣庫存
                    foreach (var detail in details)
                    {
                        var boms = _bomRepository.GetAllBoms().Where(x => x.ProductNo == detail.ProductNo).ToList();
                        foreach (var bom in boms)
                        {
                            var material = await _materialRepository.GetMaterialByMaterialNoAsync(bom.MaterialNo);
                            if (material != null)
                            {
                                material.CurrentStock -= detail.Quantity * bom.MaterialUseQuantity;
                                material.ModifyDate = DateTime.Now;
                                material.Modifier = "System";
                                await _materialRepository.UpdateMaterialAsync(material);
                            }
                        }
                    }

                    // 更新訂單狀態
                    order.Status = "生產中";
                    order.ModifyDate = DateTime.Now;
                    order.Modifier = "System";
                }
                await _context.SaveChangesAsync();


                _logger.LogInformation("成功匯出 {Count} 筆訂單到 {FilePath}", orders.Count, filePath);
                response.Success = true;
                response.Message = "匯出成功";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "匯出訂單時發生錯誤");
                response.Success = false;
                response.Message = "匯出訂單時發生錯誤";
            }
            _logger.LogTrace("離開ExportOrders");
            return response;
        }
        public async Task<BaseResponse> ExportProductAsync()
        {
            BaseResponse response = new BaseResponse();
            var baseDir = _configuration["ReportExport:BaseDir"] ?? "C:\\Users\\kiko lee\\Downloads\\Order\\Out";
            Directory.CreateDirectory(baseDir);
            var filePath = Path.Combine(baseDir, $"Product_{DateTime.Now:yyyyMMdd}.json");

            var boms = _bomRepository.GetAllBoms().ToList();
            var products = _productRepository.GetAllProducts().ToList();
            var materials = _materialRepository.GetAllMaterials().ToList();
            if (products == null)
            {
                response.Success = true;
                response.Message = "沒有產品資料";
                return response;
            }
            var exportData = products.Select(p =>
            {
                var m = boms.Where(b => b.ProductNo == p.ProductNo)
                .Select(b => new
                {
                    MaterialNo = b.MaterialNo,
                    MaterialUseQuantity = b.MaterialUseQuantity,
                    MaterialCost = materials.Where(m => m.MaterialNo == b.MaterialNo).Select(s => s.MaterialCost).FirstOrDefault()
                }).ToList();
                return new { ProductNo = p.ProductNo, Count = 0, ProductPrice = p.ProductPrice, Materials = m };
            });
            // 序列化為 JSON
            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
            response.Success = true;
            response.Message = "匯出成功";
            return response;
        }
        public async Task<BaseResponse> ExportOrdersAsync()
        {
            BaseResponse response = new BaseResponse();
            var baseDir = _configuration["ReportExport:BaseDir"] ?? "C:\\Users\\kiko lee\\Downloads\\Order\\Out";
            Directory.CreateDirectory(baseDir);
            var filePath = Path.Combine(baseDir, $"Order_{DateTime.Now:yyyyMMdd}.json");

            var orders = _orderRepository.GetAllOrders().ToList();
            if (orders == null)
            {
                response.Success = true;
                response.Message = "沒有訂單資料";
                return response;
            }
            var products = _productRepository.GetAllProducts().ToList();
            var boms = _bomRepository.GetAllBoms().ToList();
            var materials = _materialRepository.GetAllMaterials().ToList();
            var exportData = orders.Select(o => new
            {
                OrderNo = o.OrderNo,
                OrderDetails = o.OrderDetails.Select(od =>
                {
                    var product = products.FirstOrDefault(p => p.ProductNo == od.ProductNo);
                    return new
                    {
                        ProductNo = od.ProductNo,
                        Quantity = od.Quantity,
                        Price = od.Quantity * product.ProductPrice,
                        Materials = boms.Where(b => b.ProductNo == od.ProductNo)
                        .Select(b => new
                        {
                            MaterialNo = b.MaterialNo,
                            MaterialUseQuantity = b.MaterialUseQuantity,
                            MaterialCost = materials.Where(m => m.MaterialNo == b.MaterialNo).Select(s => s.MaterialCost).FirstOrDefault()
                        }).ToList()
                    };
                }).ToList()
            }).ToList();
            // 序列化為 JSON
            var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(filePath, json);
            response.Success = true;
            response.Message = "匯出成功";
            return response;
        }
    }
}
