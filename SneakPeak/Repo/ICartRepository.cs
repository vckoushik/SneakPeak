using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface ICartRepository
    {    
        Task<int> AddItem(int productId, int quantity,string size);
        Task<int> RemoveItem(int productId);
        Task<Cart> GetUserCart();
        Task<int> GetCartItemsCount(string userId = "");
        Task<bool> DoCheck();
        Task<string> GetUserEmailAsync();
    }
}