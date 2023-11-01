using Microsoft.AspNetCore.Mvc;
using SneakPeak.Data;
using SneakPeak.Repo;

namespace SneakPeak.Controllers
{
    [Route("/Wishlist")]
    public class WishlistController : Controller
    {
        private readonly SneakPeakDbContext dbContext;
        private readonly IWishlistRepository _wishlistRepo;
        public WishlistController(SneakPeakDbContext dbContext, IWishlistRepository wishlistRepo)
        {
            this.dbContext = dbContext;
            _wishlistRepo = wishlistRepo;   
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetUserWishlist")]
        public async Task<IActionResult> GetUserWishlist()
        {
            var wishlist = await _wishlistRepo.GetUserWishlist();
            return View(wishlist);
        }
        [Route("AddItem/{productId:int}")]
        public async Task<IActionResult> AddItem(int productId, int redirect = 0)
        {
            var wishlistCount = await _wishlistRepo.AddItem(productId);
            if (redirect == 0)
            {
                TempData["success"] = "Your Product Added to Wishlist";
                return Ok(wishlistCount);
            }
            return RedirectToAction("GetUserWishlist");
        }

        [Route("RemoveItem/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var wishlistCount = await _wishlistRepo.RemoveItem(productId);
            TempData["success"] = "Product Removed from Wishlist";
            return RedirectToAction("GetUserWishlist");

        }
    }
}
