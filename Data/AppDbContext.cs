using ClzProject.Models;
using ClzProject.ViewModels;
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
        //public DbSet<SellerAddCategoryViewModel> SellerAddCategoryViewModel { get; set; } = default!;
        //public DbSet<SellerCategory> SellerCategories { get; set; } = default!;
        public DbSet<FAQ> FAQs { get; set; }
        // Add this line to register the Product table
        //public DbSet<Product> Products { get; set; }
        //public DbSet<ClzProject.ViewModels.SellerAddProductViewModel> SellerAddProductViewModel { get; set; } = default!;

        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Product> Product { get; set; } = default!;


    }
}
