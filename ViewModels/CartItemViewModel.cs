using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "Rs. {0:N2}")]
        public decimal ProductPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public string Image { get; set; } = string.Empty;

        public string SellerId { get; set; } = string.Empty;

        [Display(Name = "Seller Name")]
        public string SellerName { get; set; } = string.Empty;

        // Calculated property
        public decimal SubTotal => ProductPrice * Quantity;

        // Available stock for validation
        public int AvailableStock { get; set; }
        public string SellerBusinessName { get; internal set; }
    }
}
