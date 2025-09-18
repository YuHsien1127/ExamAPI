using ExamAPI.Models;
using Microsoft.EntityFrameworkCore;

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
            return _context.Orders.Include(od => od.OrderDetails);
        }

        public async Task<Order> GetOrderByOrderNoAsync(string orderNo)
        {
            return await _context.Orders.FindAsync(orderNo);
        }
        public async Task<List<OrderDetail>> GetOrderDetailByOrderNoAsync(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
                return new List<OrderDetail>();
            return await _context.OrderDetails.Where(od => od.OrderNo == orderNo).ToListAsync();
        }
        public async Task AddOrderAsync(Order order, List<OrderDetail> orderDetail)
        {
            await _context.Orders.AddAsync(order);
            await _context.OrderDetails.AddRangeAsync(orderDetail);
        }

        public async Task CancelOrderAsync(Order order, List<OrderDetail> orderDetail)
        {
            _context.Orders.Remove(order);
            _context.OrderDetails.RemoveRange(orderDetail);
            await Task.CompletedTask;
        }

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }
    }
}
