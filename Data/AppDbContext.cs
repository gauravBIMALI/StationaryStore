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
        public DbSet<AdminCommission> AdminCommissions { get; set; } = default!;
        public DbSet<ChatBotFAQ> ChatBotFAQs { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Product> Product { get; set; } = default!;
        public object Products { get; internal set; }

        public DbSet<ClzProject.Models.AdminContact> AdminContact { get; set; } = default!;
        public DbSet<ClzProject.Models.BuyerContactMessage> BuyerContactMessages { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;

        public object Categories { get; internal set; }

        // Comment feature
        public DbSet<ProductComment> ProductComments { get; set; } = default!;
        public DbSet<ProductCommentReply> ProductCommentReplies { get; set; } = default!;
        //Cart feature
        public DbSet<Cart> Carts { get; set; } = default!;
        public DbSet<SellerNotification> SellerNotifications { get; set; } = default!;
        public DbSet<EsewaPayment> EsewaPayments { get; set; } = default!;

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

            //Configure Cart relationships
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

            // Order configurations
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany()
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Seller)
                .WithMany()
                .HasForeignKey(oi => oi.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification configuration
            modelBuilder.Entity<SellerNotification>()
                .HasOne(n => n.Seller)
                .WithMany()
                .HasForeignKey(n => n.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SellerNotification>()
                .HasOne(n => n.Order)
                .WithMany()
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AdminCommission>()
        .HasOne(c => c.Order)
        .WithMany()
        .HasForeignKey(c => c.OrderId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminCommission>()
                .HasOne(c => c.OrderItem)
                .WithMany()
                .HasForeignKey(c => c.OrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminCommission>()
                .HasOne(c => c.Seller)
                .WithMany()
                .HasForeignKey(c => c.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdminCommission>()
                .HasOne(c => c.Buyer)
                .WithMany()
                .HasForeignKey(c => c.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EsewaPayment>()
       .HasOne(ep => ep.Order)
       .WithMany()
       .HasForeignKey(ep => ep.OrderId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EsewaPayment>()
                .HasOne(ep => ep.Buyer)
                .WithMany()
                .HasForeignKey(ep => ep.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}