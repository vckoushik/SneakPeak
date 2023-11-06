using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class WishlistItems
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public int WishlistId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public Product Product { get; set; }
    }
}
