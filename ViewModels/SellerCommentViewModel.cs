using ClzProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class SellerCommentViewModel
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool HasReply { get; set; }
        public string? ReplyText { get; set; }
        public DateTime? ReplyDate { get; set; }
    }

    public class SellerReplyViewModel
    {
        [Required(ErrorMessage = "Reply is required")]
        [StringLength(1000, ErrorMessage = "Reply cannot exceed 1000 characters")]
        public string ReplyText { get; set; } = null!;

        public int CommentId { get; set; }
    }

    public class SellerProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string CategoryType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int CommentsCount { get; set; }
        public int UnrepliedCommentsCount { get; set; }
    }

    public class SellerDashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int PendingComments { get; set; }
        public int TotalComments { get; set; }
        public List<SellerCommentViewModel> RecentComments { get; set; } = new List<SellerCommentViewModel>();
        public List<SellerProductViewModel> RecentProducts { get; set; } = new List<SellerProductViewModel>();

        // NEW: Orders Stats
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<SellerOrderViewModel> RecentOrders { get; set; } = new List<SellerOrderViewModel>();

        // NEW: Notifications
        public int UnreadNotifications { get; set; }
        public List<SellerNotification> RecentNotifications { get; set; } = new List<SellerNotification>();

    }
}