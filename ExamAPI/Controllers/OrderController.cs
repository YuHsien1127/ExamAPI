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
    [Authorize(Roles = "admin, customer")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 查詢訂單
        /// </summary>
        /// <param name="page">頁碼</param>
        /// <param name="pageSize">一頁幾筆</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<OrderResponse> GetAllOrdersAsync(int page = 1, int pageSize = 10)
        {
            return await _orderService.GetAllOrdersAsync(page, pageSize);
        }

        /// <summary>
        /// 新增訂單
        /// </summary>
        /// <param name="orderRequest">訂單資料</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<OrderResponse> AddOrderAsync([FromBody] OrderRequest orderRequest)
        {
            return await _orderService.AddOrderAsync(orderRequest);
        }

        /// <summary>
        /// 修改訂單
        /// </summary>
        /// <param name="orderRequest">訂單資料</param>
        /// <param name="serialNo">流水號</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<OrderResponse> UpdateOrderAsync([FromBody] OrderRequest orderRequest, int serialNo = 0)
        {
            return await _orderService.UpdateOrderAsync(serialNo, orderRequest);
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="serialNo">流水號</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<OrderResponse> CancelOrderAsync(int serialNo = 0)
        {
            return await _orderService.CancelOrderAsync(serialNo);
        }
    }
}
