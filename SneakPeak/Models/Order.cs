using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SneakPeak.Models
{
    public class Order
    {
        // Unique identifier for the order
        public int Id { get; set; }
        [Required] 
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;  
        
        public string OrderStatus { get; set; }

        public bool isDeleted { get; set; } = false;

        public ICollection<OrderLineItem> LineItems { get; set; }
    }

}
