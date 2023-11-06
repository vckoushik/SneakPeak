
using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        // Price of the product
        [Required]
        public decimal Price { get; set; }

        // Size of the product (applicable to sneakers)
        public string Size { get; set; }

        // Brand of the product (e.g., Nike, Adidas)
        public string Brand { get; set; }

        // Color of the product (e.g., Red, Blue)
        public string Color { get; set; }

        // Availability status of the product (e.g., In Stock, Out of Stock)
        public bool IsInStock { get; set; }

        public string ImageUrl { get; set; }

        public string Category { get; set; }

    }
}
