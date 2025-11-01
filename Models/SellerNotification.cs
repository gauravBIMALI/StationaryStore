using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserRoles.Models;

namespace ClzProject.Models
{
    [Table("SellerNotifications")]
    public class SellerNotification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public string SellerId { get; set; } = string.Empty;

        [Required]
        public string NotificationType { get; set; } = string.Empty; // NewOrder, OrderCancelled, etc.

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public int? OrderItemId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        [ForeignKey("SellerId")]
        public virtual Users? Seller { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
