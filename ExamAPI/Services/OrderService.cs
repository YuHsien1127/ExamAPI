using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExamAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ExamSQLContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;
        public OrderService(ExamSQLContext context, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, ILogger<OrderService> logger, IProductRepository productRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _logger = logger;
            _productRepository = productRepository;
        }

        public OrderResponse GetAllOrders(int page, int pageSize)
        {
            _logger.LogTrace("進入GetAllOrdersAsync");
            OrderResponse response = new OrderResponse();
            var orders = _orderRepository.GetAllOrders();
            var query = orders.Include(od => od.OrderDetails);
            var o = query.Select(x => new OrderDto
            {
                OrderNo = x.OrderNo,
                OrderSubject = x.OrderSubject,
                OrderApplicant = x.OrderApplicant,
                Status = x.Status,
                orderDetails = x.OrderDetails.Select(y => new OrderDetailDTO
                {
                    SerialNo = y.SerialNo,
                    OrderNo = y.OrderNo,
                    ProductNo = y.ProductNo,
                    Quantity = y.Quantity
                }).ToList()
            });
            _logger.LogDebug("取得訂單數量：{o.Count()}", o.Count());
            var pagedList = o.ToPagedList(page, pageSize);
            response.Orders = pagedList.ToList();
            response.PageCount = pagedList.Count;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = "查詢成功";
            _logger.LogTrace("離開GetAllOrdersAsync");
            return response;
        }

        // 需區分訂單作業狀況，剛新增訂單為成立
        public async Task<OrderResponse> AddOrderAsync(OrderRequest orderRequest)
        {
            _logger.LogTrace("進入AddOrderAsync");
            OrderResponse response = new OrderResponse();
            try
            {
                if (orderRequest == null)
                {
                    _logger.LogWarning("新增訂單資料為空");
                    response.Success = false;
                    response.Message = "新增訂單資料為空";
                    _logger.LogTrace("離開AddOrder");
                    return response;
                }
                if (orderRequest.Details.Any(p => string.IsNullOrEmpty(p.ProductNo))
                    || orderRequest.Details.Any(q => q.Quantity == 0))
                {
                    _logger.LogWarning("必填欄位不能為空");
                    response.Success = false;
                    response.Message = "必填欄位不能為空";
                    _logger.LogTrace("離開AddOrder");
                    return response;
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                var today = DateTime.Now.ToString("yyyyMMdd");
                // 找到今天最後一筆訂單
                var lastOrder = await _context.Orders.OrderByDescending(o => o.OrderNo).FirstOrDefaultAsync();
                int number = 1;
                if (lastOrder != null)
                {
                    // 取出流水號部分
                    string lastOrderNo = lastOrder.OrderNo.Substring(9);
                    number = int.Parse(lastOrderNo) + 1;
                }
                var order = new Order
                {
                    OrderNo = $"O{today}{number:D5}",
                    OrderSubject = orderRequest.OrderSubject,
                    OrderApplicant = user.UserName,
                    Status = "成立",
                    CreateDate = DateTime.Now,
                    Creator = user.UserName,
                    ModifyDate = DateTime.Now,
                    Modifier = "System"
                };
                await _orderRepository.AddOrderAsync(order);
                var orderDetail = orderRequest.Details.Select(od => new OrderDetail
                {
                    OrderNo = order.OrderNo,
                    ProductNo = od.ProductNo,
                    Quantity = od.Quantity,
                    CreateDate = DateTime.Now,
                    Creator = user.UserName,
                    ModifyDate = DateTime.Now,
                    Modifier = "System"
                }).ToList();
                _logger.LogTrace("準備新增訂單明細，共{Count}筆", orderDetail.Count);
                await _orderRepository.AddOrderDetailAsync(orderDetail);
                int count = _context.SaveChanges();
                if(count > 0)
                {
                    var o = new OrderDto
                    {
                        OrderNo = order.OrderNo,
                        OrderSubject = order.OrderSubject,
                        OrderApplicant = order.OrderApplicant,
                        Status = order.Status,
                        orderDetails = orderDetail.Select(od=>new OrderDetailDTO
                        {
                            SerialNo = od.SerialNo,
                            OrderNo = od.OrderNo,
                            ProductNo = od.ProductNo,
                            Quantity = od.Quantity
                        }).ToList()
                    };
                    _logger.LogInformation("成功新增訂單 {OrderNo}", o.OrderNo);
                    response.Orders = new List<OrderDto> { o };
                    response.Success = true;
                    response.Message = "新增訂單成功";
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("新增訂單發生錯誤");
                response.Success = false;
                response.Message = "新增訂單發生錯誤";
            }
            _logger.LogTrace("離開AddOrderAsync");
            return response;
        }

        // 訂單改狀態為生產中時需檢查產品的物料表是否有庫存充足，若不足則不可以轉為生產
        // 當使用改為生產中須將該訂單的所有產品用到的數量扣掉庫存
        public async Task<OrderResponse> UpdateOrderAsync(int serialNo, OrderRequest orderRequest)
        {
            _logger.LogTrace("進入UpdateOrderAsync");
            OrderResponse response = new OrderResponse();
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogWarning("修改訂單發生錯誤");
                response.Success = false;
                response.Message = "修改訂單發生錯誤";
            }
            _logger.LogTrace("離開UpdateOrderAsync");
            return response;
        }
        public async Task<OrderResponse> CancelOrderAsync(int serialNo)
        {
            _logger.LogTrace("進入CancelOrderAsync");
            OrderResponse response = new OrderResponse();
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogWarning("取消訂單發生錯誤");
                response.Success = false;
                response.Message = "取消訂單發生錯誤";
            }
            _logger.LogTrace("離開CancelOrderAsync");
            return response;
        }
    }
}
