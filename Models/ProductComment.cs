using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserRoles.Models;

namespace ClzProject.Models
{
    public class ProductComment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string CommentText { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public virtual ProductCommentReply? Reply { get; set; }
    }

    public class ProductCommentReply
    {
        [Key]
        public int ReplyId { get; set; }

        [Required]
        public int CommentId { get; set; }

        [Required]
        public string SellerId { get; set; } = null!;

        [Required]
        [StringLength(1000, ErrorMessage = "Reply cannot exceed 1000 characters")]
        public string ReplyText { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("CommentId")]
        public virtual ProductComment Comment { get; set; } = null!;

        [ForeignKey("SellerId")]
        public virtual Users Seller { get; set; } = null!;
    }
}

// ViewModels for comments
namespace UserRoles.ViewModels
{
    public class AddCommentViewModel
    {
        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string CommentText { get; set; } = null!;

        public int ProductId { get; set; }
    }

    public class AddReplyViewModel
    {
        [Required(ErrorMessage = "Reply is required")]
        [StringLength(1000, ErrorMessage = "Reply cannot exceed 1000 characters")]
        public string ReplyText { get; set; } = null!;

        public int CommentId { get; set; }
    }

    public class CommentDisplayViewModel
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ReplyDisplayViewModel? Reply { get; set; }
    }

    public class ReplyDisplayViewModel
    {
        public int ReplyId { get; set; }
        public string ReplyText { get; set; } = null!;
        public string SellerName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}