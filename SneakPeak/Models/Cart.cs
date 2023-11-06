using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class Cart
    {
        // Unique identifier for the cart
        [Key]
        public int Id { get; set; }

        // User ID (foreign key) - used to associate the cart with a user (optional)
        public string UserId { get; set; }

        // List of cart items
        public ICollection<CartItem> Items { get; set; }

        // Total number of items in the cart (calculated property)
        public int TotalItems => Items?.Sum(item => item.Quantity) ?? 0;

        // Total amount for all cart items (calculated property)
        public decimal TotalAmount => Items?.Sum(item => item.TotalAmount) ?? 0;
    }
}
