using ExamAPI.Models;

namespace ExamAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ExamSQLContext _context;
        public OrderRepository(ExamSQLContext context)
        {
            _context = context;
        }

        public IQueryable<Order> GetAllOrders()
        {
            return _context.Orders;
        }

        public async Task<Order> GetOrderByOrderNoAsync(string orderNo)
        {
            return await _context.Orders.FindAsync(orderNo);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }
        public async Task AddOrderDetailAsync(List<OrderDetail> orderDetail)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetail);
        }

        public async Task CancelOrderAsync(Order order)
        {
            _context.Orders.Remove(order);
            await Task.CompletedTask;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }
    }
}
