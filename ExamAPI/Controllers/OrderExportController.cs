using ExamAPI.Dto.Response;
using ExamAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrderExportController : ControllerBase
    {
        private readonly IOrderExportService _orderExportService;
        public OrderExportController(IOrderExportService orderExportService)
        {
            _orderExportService = orderExportService;
        }

        /// <summary>
        /// 手動觸發訂單匯出
        /// </summary>
        [HttpPost]
        public async Task<BaseResponse> Production_ExportOrdersAsync()
        {
            return await _orderExportService.Production_ExportOrdersAsync();
        }
        /// <summary>
        /// 手動觸發產品物料明細匯出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> ExportProductAsync()
        {
            return await _orderExportService.ExportProductAsync();
        }
        /// <summary>
        /// 手動觸發訂單明細匯出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<BaseResponse> ExportOrdersAsync()
        {
            return await _orderExportService.ExportOrdersAsync();
        }
    }
}