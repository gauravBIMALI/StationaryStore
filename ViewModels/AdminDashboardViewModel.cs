using ClzProject.Models;
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


        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public decimal TotalRevenue { get; set; } // Total sales amount
        public decimal TotalCommissionEarned { get; set; } // Admin's 5% commission
        public decimal PendingCommission { get; set; } // From pending/shipped orders
        public decimal CompletedCommission { get; set; } // From delivered orders
        public List<AdminCommission>? RecentCommissions { get; set; }

        // Commission breakdown by status
        public decimal TodayCommission { get; set; }
        public decimal ThisMonthCommission { get; set; }
    }
}
