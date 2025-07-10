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
        public DbSet<ClzProject.ViewModels.SellerAddCategoryViewModel> SellerAddCategoryViewModel { get; set; } = default!;
        public DbSet<ClzProject.Models.SellerCategory> SellerCategories { get; set; } = default!;
    }
}
