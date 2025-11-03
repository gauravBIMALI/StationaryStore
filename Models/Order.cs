using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserRoles.Models;

namespace ClzProject.Models
{
    [Table("Orders")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public string BuyerId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; } = 60;

        [Required]
        public string PaymentMethod { get; set; } = string.Empty; // COD, Esewa

        [Required]
        public string OrderStatus { get; set; } = "Pending";

        public string? PaymentStatus { get; set; } = "Pending";
        [Required]
        public string DeliveryName { get; set; } = string.Empty;

        [Required]
        public string DeliveryPhone { get; set; } = string.Empty;

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public string DeliveryCity { get; set; } = string.Empty;

        public string? DeliveryState { get; set; }

        public string? DeliveryNote { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? DeliveredDate { get; set; }

        // Navigation Properties
        [ForeignKey("BuyerId")]
        public virtual Users? Buyer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
