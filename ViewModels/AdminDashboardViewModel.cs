using UserRoles.Models;

namespace ClzProject.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalSellers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalFAQs { get; set; }
        public int TotalUsers { get; set; }

        // Recent activities for dashboard
        public IEnumerable<dynamic>? RecentProducts { get; set; }
        public IEnumerable<Users>? RecentSellers { get; set; }
    }
}
