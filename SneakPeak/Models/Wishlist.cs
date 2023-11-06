using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }

        // User ID (foreign key) - used to associate the cart with a user (optional)
        public string UserId { get; set; }

        // List of cart items
        public ICollection<WishlistItems> Items { get; set; }
    }
}
