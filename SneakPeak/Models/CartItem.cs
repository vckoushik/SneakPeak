using Microsoft.Build.Framework;

namespace SneakPeak.Models
{
    public class CartItem
    {
        // Unique identifier for the cart item
        public int Id { get; set; }
        [Required]

        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; }

        // Quantity of the product in the cart item
        public int Quantity { get; set; }

        // Price per unit for the product
        public decimal PricePerUnit { get; set; }

        // Total amount for the cart item (calculated property)
        public decimal TotalAmount => Quantity * PricePerUnit;
    }
}
