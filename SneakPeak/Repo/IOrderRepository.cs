using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
        Task<Order> UserOrdersById(int OrderId);
        Task<IEnumerable<Order>> GetOrders();
        Task<Order> OrdersById(int OrderId);
    }
}