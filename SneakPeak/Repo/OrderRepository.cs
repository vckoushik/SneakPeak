using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Data;
using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public class OrderRepository:IOrderRepository
    {
        private readonly SneakPeakDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<SneakPeakUser> _userManager;


        public OrderRepository(SneakPeakDbContext db,
            UserManager<SneakPeakUser> userManager,
             IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Order>> UserOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            var orders = await _db.Order
                            .Include(x => x.LineItems)
                            .ThenInclude(x => x.product)
                            .Where(a => a.UserId == userId)
                            .ToListAsync();
            orders.Reverse();
            return orders;
        }
        public async Task<IEnumerable<Order>> GetOrders()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            var orders = await _db.Order
                            .Include(x => x.LineItems)
                            .ThenInclude(x => x.product)
                            .ToListAsync();
            orders.Reverse();
            return orders;
        }

        public async Task<Order> UserOrdersById(int OrderId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("User is not logged-in");
            var order = await _db.Order
                            .Include(x => x.LineItems)
                            .ThenInclude(x => x.product)
                            .Where(a => a.UserId == userId && a.Id == OrderId)
                            .FirstOrDefaultAsync();
                       
            return order;
        }

        public async Task<Order> OrdersById(int OrderId)
        {
   
            var order = await _db.Order
                            .Include(x => x.LineItems)
                            .ThenInclude(x => x.product)
                            .Where(a=>a.Id == OrderId)
                            .FirstOrDefaultAsync();

            return order;
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
           
            return userId;
        }
    }
}

