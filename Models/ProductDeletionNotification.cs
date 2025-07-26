using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.Models
{
    [Table("ProductDeletionNotifications")]
    public class ProductDeletionNotification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public string SellerId { get; set; } = string.Empty;

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string DeletionReason { get; set; } = string.Empty;

        [Required]
        public string AdminName { get; set; } = string.Empty;

        public DateTime DeletedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}