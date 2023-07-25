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
            return orders;
        }

        private string GetUserId()
        {
            var principal = _httpContextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}

