namespace ClzProject.ViewModels
{
    public class ProductDisplayViewModel
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string ImageBase64 { get; set; } = string.Empty; // Base64 image string

        // Seller Info
        public string SellerName { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

}
