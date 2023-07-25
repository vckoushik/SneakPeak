using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders();
    }
}