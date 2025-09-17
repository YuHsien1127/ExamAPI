using ExamAPI.Dto.Request;
using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public interface IOrderRepository
    {
        public IQueryable<Order> GetAllOrders();
        public Task<Order> GetOrderByOrderNoAsync(string orderNo);
        public Task AddOrderAsync(Order order);
        public Task AddOrderDetailAsync(List<OrderDetail> orderDetail);
        public Task UpdateOrderAsync(Order order);
        public Task CancelOrderAsync(Order order);
    }
}
