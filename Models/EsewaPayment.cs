using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserRoles.Models;

namespace ClzProject.Models
{
    [Table("EsewaPayments")]
    public class EsewaPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public string TransactionId { get; set; } = string.Empty; // Unique ID for this payment

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ServiceCharge { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryCharge { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public string ProductId { get; set; } = string.Empty; // Order number as product ID

        [Required]
        public string BuyerId { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        public string? EsewaTransactionCode { get; set; } // eSewa's reference number

        public string? RefId { get; set; } // eSewa reference ID

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? CompletedAt { get; set; }

        // Navigation Properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("BuyerId")]
        public virtual Users? Buyer { get; set; }
    }
}
