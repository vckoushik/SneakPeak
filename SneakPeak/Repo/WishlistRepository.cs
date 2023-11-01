using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Data;
using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public class WishlistRepository : IWishlistRepository
    {
       
        private readonly SneakPeakDbContext _context;
        private readonly UserManager<SneakPeakUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WishlistRepository(SneakPeakDbContext context, UserManager<SneakPeakUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = contextAccessor;

        }
        public async Task<int> AddItem(int productId)
        {
            string userId = GetUserId();
            var transaction = _context.Database.BeginTransaction();
            try
            {

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid user id");
                }
                var wishlist = await GetWishlist(userId);
                if (wishlist == null)
                {
                    wishlist = new Wishlist()
                    {
                        UserId = userId
                    };
                    _context.Wishlist.Add(wishlist);
                }
                _context.SaveChanges();
                //For existing cart if we have a item in cart
                var wishlistItem = _context.WishlistItems.FirstOrDefault(a => a.WishlistId == wishlist.Id && a.ProductId == productId);
                if (wishlistItem is not null)
                {
                    // Already wishlisted item
                    var total = await GetWishlistItemsCount(userId);
                    return total;
                }
                else
                {
                    var product = _context.Product.Find(productId);
                    wishlistItem = new WishlistItems()
                    {
                        ProductId = productId,
                        WishlistId = wishlist.Id,
                        Product = product
                    };
                    _context.WishlistItems.Add(wishlistItem);
                }
                _context.SaveChanges();
                transaction.Commit();


            }
            catch (Exception ex)
            {
            }
            var totalItems = await GetWishlistItemsCount(userId);
            return totalItems;
        
        }

        public async Task<int> RemoveItem(int productId)
        {
            string userId = GetUserId();
            try
            {

                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid user id");
                }
                var wishlist = await GetWishlist(userId);
                if (wishlist == null)
                {
                    throw new Exception("Invalid wishlist");
                }
                _context.SaveChanges();
                //For existing cart if we have a item in cart
                var wishlistItem = _context.WishlistItems.FirstOrDefault(a => a.WishlistId == wishlist.Id && a.ProductId == productId);
                if (wishlistItem is null)
                {
                    throw new Exception("No Items in Wishlist ");
                }
                else
                {
                    _context.WishlistItems.Remove(wishlistItem);

                }
                _context.SaveChanges();


            }
            catch (Exception ex)
            {
            }
            var totalItems = await GetWishlistItemsCount(userId);
            return totalItems;

        }

        public async Task<Wishlist> GetUserWishlist()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new Exception("invalid user");
            }
            var wishlist = await _context.Wishlist.Include(a => a.Items)
                                           .ThenInclude(a => a.Product)
                                           .Where(a => a.UserId == userId).FirstOrDefaultAsync();

            return wishlist;


        }

        private async Task<Wishlist?> GetWishlist(string userId)
        {
            var result = _context.Wishlist.FirstOrDefault(x => x.UserId == userId);
            return result;
        }

        public async Task<int> GetWishlistItemsCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }
            var data = await (from wishlist in _context.Wishlist
                              join wishlistItem in _context.WishlistItems
                              on wishlist.Id equals wishlistItem.WishlistId
                              where wishlist.UserId == userId
                              select new { wishlistItem.Id }).ToListAsync();
            return data.Count;
        }

        private string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(user);
            return userId;

        }
    }
}
