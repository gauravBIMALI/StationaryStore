using UserRoles.Models;

namespace ClzProject.ViewModels
{
    public class ProductDisplayViewModel
    {
        public int ProductID { get; set; }


        public string SellerID { get; set; } = string.Empty;


        public string ProductName { get; set; } = string.Empty;

        public string ProductDescription { get; set; } = string.Empty;

        public decimal ProductPrice { get; set; }

        public int ProductQuantity { get; set; }

        public string CategoryType { get; set; } = string.Empty;

        public string ImageBase64 { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public string SellerId { get; set; } = string.Empty;

        // Navigation Property
        public Users Seller { get; set; } = new Users();

        // Additional for admin view
        public string SellerName { get; set; } = string.Empty;

        public string SellerEmail { get; set; } = string.Empty;

        // (Later) For buyer side, you may add:
        // public bool IsAvailable { get; set; }
    }

}
