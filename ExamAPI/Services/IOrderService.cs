using ExamAPI.Dto.Request;
using ExamAPI.Dto.Response;

namespace ExamAPI.Services
{
    public interface IOrderService
    {
        public OrderResponse GetAllOrders(int page, int pageSize);
        public Task<OrderResponse> AddOrderAsync(OrderRequest orderRequest);
        public Task<OrderResponse> UpdateOrderAsync(int serialNo, OrderRequest orderRequest);
        public Task<OrderResponse> CancelOrderAsync(int serialNo);
    }
}
