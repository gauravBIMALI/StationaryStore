using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserRoles.Models;

namespace ClzProject.Models
{
    public class AdminCommission
    {
        [Key]
        public int CommissionId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int OrderItemId { get; set; }

        [Required]
        public string SellerId { get; set; } = string.Empty;

        [Required]
        public string BuyerId { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductAmount { get; set; } // Product price × quantity

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionRate { get; set; } = 5.0m; // Default 5%

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CommissionAmount { get; set; } // Calculated commission

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SellerEarning { get; set; } // Amount seller receives

        [Required]
        public string OrderStatus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; } // When order is delivered

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("OrderItemId")]
        public virtual OrderItem? OrderItem { get; set; }

        [ForeignKey("SellerId")]
        public virtual Users? Seller { get; set; }

        [ForeignKey("BuyerId")]
        public virtual Users? Buyer { get; set; }

    }
}
