using ClzProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserRoles.Models;

namespace UserRoles.Data
{
    public class AppDbContext : IdentityDbContext<Users>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        //DbSets
        public DbSet<ChatBotFAQ> ChatBotFAQs { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Product> Product { get; set; } = default!;
        public object Products { get; internal set; }
        public DbSet<ProductDeletionNotification> ProductDeletionNotifications { get; set; }
        public DbSet<ClzProject.Models.AdminContact> AdminContact { get; set; } = default!;
        public DbSet<ClzProject.Models.BuyerContactMessage> BuyerContactMessages { get; set; } = default!;
        public object Categories { get; internal set; }

        // Comment feature
        public DbSet<ProductComment> ProductComments { get; set; } = default!;
        public DbSet<ProductCommentReply> ProductCommentReplies { get; set; } = default!;

        // ADD THIS - Cart feature
        public DbSet<Cart> Carts { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for comments
            modelBuilder.Entity<ProductComment>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductComment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductCommentReply>()
                .HasOne(r => r.Comment)
                .WithOne(c => c.Reply)
                .HasForeignKey<ProductCommentReply>(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCommentReply>()
                .HasOne(r => r.Seller)
                .WithMany()
                .HasForeignKey(r => r.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ADD THIS - Configure Cart relationships
            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.BuyerId, c.ProductId })
                .IsUnique();

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Buyer)
                .WithMany()
                .HasForeignKey(c => c.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}