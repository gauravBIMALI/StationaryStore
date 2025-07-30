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
        public DbSet<ProductDeletionNotification> ProductDeletionNotifications { get; set; }
        public DbSet<ClzProject.Models.AdminContact> AdminContact { get; set; } = default!;
        public object Categories { get; internal set; }
        //public object Categories { get;  set; }
    }
}
