using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class OrderLineItem
    {
        // Unique identifier for the order line item
        public int Id { get; set; }

        // Order ID (foreign key) - used to associate line item with an order
        public int OrderId { get; set; }

        public Order Order { get; set; }

        public Product product{ get; set; }
        [Required]
        [MaxLength(100)]
        public int ProductId { get; set; }

        // Quantity of the product in the line item
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        // Price per unit for the product
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerUnit { get; set; }

        // Total amount for the line item (calculated property)
        [NotMapped] // This attribute indicates that this property is not mapped to a database column
        public decimal TotalAmount => Quantity * PricePerUnit;
    }

}
