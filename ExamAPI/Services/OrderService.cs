using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;
using ExamAPI.Models;
using ExamAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace ExamAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ExamSQLContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IMaterialRepository _materialRepository;
        private readonly IBomRepository _bomRepository;
        private readonly IEmailService _emailService;
        public OrderService(ExamSQLContext context, IOrderRepository orderRepository, IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository, ILogger<OrderService> logger, IMaterialRepository materialRepository
            , IBomRepository bomRepository, IEmailService emailService)
        {
            _context = context;
            _orderRepository = orderRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _logger = logger;
            _materialRepository = materialRepository;
            _bomRepository = bomRepository;
            _emailService = emailService;
        }

        public OrderResponse GetAllOrders(int page, int pageSize)
        {
            _logger.LogTrace("進入GetAllOrdersAsync");
            OrderResponse response = new OrderResponse();
            var orders = _orderRepository.GetAllOrders().Select(x => new OrderDto
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
            _logger.LogDebug("取得訂單數量：{o.Count()}", orders.Count());
            var pagedList = orders.ToPagedList(page, pageSize);
            response.Orders = pagedList.ToList();
            response.PageCount = pagedList.PageCount;
            response.TotalCount = pagedList.TotalItemCount;
            response.Success = true;
            response.Message = $"取得第{page}頁，{pageSize}筆資料";
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
                await _orderRepository.AddOrderAsync(order, orderDetail);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var o = new OrderDto
                    {
                        OrderNo = order.OrderNo,
                        OrderSubject = order.OrderSubject,
                        OrderApplicant = order.OrderApplicant,
                        Status = order.Status,
                        orderDetails = orderDetail.Select(od => new OrderDetailDTO
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
                    
                    var emailRequest = new EmailRequest
                    {
                        ToEmail = user.UserEmail,
                        ToName = user.UserName,
                        Subject = "訂單確認通知",
                        Body = $"訂單編號 {order.OrderNo} 成立",
                        IsHtml = false // 純文字郵件
                    };

                    // 寄送確認郵件
                    var emailResponse = await _emailService.SendEmailAsync(emailRequest);

                    if (!emailResponse.Success)
                    {
                        _logger.LogWarning("訂單確認郵件寄送失敗：{Message}", emailResponse.Message);
                    }                    
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
        public async Task<OrderResponse> UpdateOrderAsync(string orderNo)
        {
            _logger.LogTrace("進入UpdateOrderAsync");
            OrderResponse response = new OrderResponse();
            try
            {
                if (string.IsNullOrEmpty(orderNo))
                {
                    _logger.LogWarning("訂單編號為空");
                    response.Success = false;
                    response.Message = "訂單編號為空";
                    _logger.LogTrace("離開UpdateOrderAsync");
                    return response;
                }
                var existOrder = await _orderRepository.GetOrderByOrderNoAsync(orderNo);
                if (existOrder == null)
                {
                    _logger.LogWarning("訂單{OrderNo}不存在", orderNo);
                    response.Success = false;
                    response.Message = "訂單不存在";
                    _logger.LogTrace("離開UpdateOrderAsync");
                    return response;
                }
                var existDetails = await _orderRepository.GetOrderDetailByOrderNoAsync(orderNo);
                if (existDetails == null || !existDetails.Any())
                {
                    _logger.LogWarning("訂單 {OrderNo} 沒有明細", orderNo);
                    response.Success = false;
                    response.Message = "訂單沒有明細";
                    return response;
                }
                // 先檢查所有物料庫存是否足夠
                foreach (var detail in existDetails)
                {
                    var boms = _bomRepository.GetAllBoms().Where(x => x.ProductNo == detail.ProductNo).ToList();
                    if (!boms.Any())
                    {
                        _logger.LogWarning("產品{ProductNo}沒有Bom", detail.ProductNo);
                        response.Success = false;
                        response.Message = $"產品{detail.ProductNo}沒有Bom";
                        return response;
                    }
                    foreach (var bom in boms)
                    {
                        var material = await _materialRepository.GetMaterialByMaterialNoAsync(bom.MaterialNo);
                        if (material == null)
                        {
                            _logger.LogWarning("物料{MaterialNo}不存在", bom.MaterialNo);
                            response.Success = false;
                            response.Message = $"物料{bom.MaterialNo}不存在";
                            return response;
                        }
                        if (material.CurrentStock < detail.Quantity * bom.MaterialUseQuantity)
                        {
                            _logger.LogWarning("物料{MaterialNo}庫存不足，需要{Required}，目前{Stock}"
                                , bom.MaterialNo, detail.Quantity * bom.MaterialUseQuantity, material.CurrentStock);
                            response.Success = false;
                            response.Message = $"物料{bom.MaterialNo}庫存不足";
                            return response;
                        }
                    }
                }
                var userAccount = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userRepository.GetUserByUserAccountAsync(userAccount);
                // 更新Material庫存
                foreach (var detail in existDetails)
                {
                    var boms = _bomRepository.GetAllBoms().Where(x => x.ProductNo == detail.ProductNo).ToList();
                    foreach (var bom in boms)
                    {
                        var material = await _materialRepository.GetMaterialByMaterialNoAsync(bom.MaterialNo);

                        material.CurrentStock -= detail.Quantity * bom.MaterialUseQuantity;
                        material.ModifyDate = DateTime.Now;
                        material.Modifier = user.UserName;

                        await _materialRepository.UpdateMaterialAsync(material);
                    }
                }
                existOrder.Status = "生產中";
                existOrder.ModifyDate = DateTime.Now;
                existOrder.Modifier = user.UserName;
                await _orderRepository.UpdateOrderAsync(existOrder);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    var o = new OrderDto
                    {
                        OrderNo = existOrder.OrderNo,
                        OrderSubject = existOrder.OrderSubject,
                        OrderApplicant = existOrder.OrderApplicant,
                        Status = existOrder.Status,
                        orderDetails = existDetails.Select(od => new OrderDetailDTO
                        {
                            SerialNo = od.SerialNo,
                            OrderNo = od.OrderNo,
                            ProductNo = od.ProductNo,
                            Quantity = od.Quantity
                        }).ToList()
                    };
                    _logger.LogInformation("成功訂單物料{OrderNo}", orderNo);
                    response.Orders = new List<OrderDto> { o };
                    response.Success = true;
                    response.Message = "修改訂單成功";
                }
                else
                {
                    _logger.LogWarning("修改訂單失敗");
                    response.Success = false;
                    response.Message = "修改訂單失敗";
                }
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
        public async Task<OrderResponse> CancelOrderAsync(string orderNo)
        {
            _logger.LogTrace("進入CancelOrderAsync");
            OrderResponse response = new OrderResponse();
            try
            {
                if (string.IsNullOrEmpty(orderNo))
                {
                    _logger.LogWarning("訂單編號為空");
                    response.Success = false;
                    response.Message = "訂單編號為空";
                    _logger.LogTrace("離開CancelOrderAsync");
                    return response;
                }
                var order = await _orderRepository.GetOrderByOrderNoAsync(orderNo);
                var detail = await _orderRepository.GetOrderDetailByOrderNoAsync(orderNo);
                if (order == null || detail == null)
                {
                    _logger.LogWarning("訂單{OrderNo}不存在", orderNo);
                    response.Success = false;
                    response.Message = "訂單不存在";
                    _logger.LogTrace("離開CancelOrderAsync");
                    return response;
                }
                order.Status = "取消";
                await _orderRepository.UpdateOrderAsync(order);
                int count = _context.SaveChanges();
                if (count > 0)
                {
                    _logger.LogInformation("成功取消訂單{OrderNo}", orderNo);
                    response.Success = true;
                    response.Message = "取消成功";
                }
                else
                {
                    _logger.LogWarning("取消訂單失敗{OrderNo}", orderNo);
                    response.Success = false;
                    response.Message = "取消失敗";
                }
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
