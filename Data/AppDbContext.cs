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
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Product> Product { get; set; } = default!;
        public object Products { get; internal set; }

        //public DbSet<Product> Products { get; set; } = default!;

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    // Configure FK between Product and Users
        //    builder.Entity<Product>()
        //           .HasOne(p => p.Seller)
        //           .WithMany()
        //           .HasForeignKey(p => p.SellerId)
        //           .OnDelete(DeleteBehavior.Restrict);
        //}
    }
}
