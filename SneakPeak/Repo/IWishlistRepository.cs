using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface IWishlistRepository
    {
        Task<int> AddItem(int productId);
        Task<int> RemoveItem(int productId);
        Task<Wishlist> GetUserWishlist();
    }
}
