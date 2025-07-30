using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        [DisplayFormat(DataFormatString = "Rs. {0:N2}")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Quantity in Stock")]
        public int ProductQuantity { get; set; }

        [Display(Name = "Category")]
        public string CategoryType { get; set; } = string.Empty;

        public string Image { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public string SellerId { get; set; } = string.Empty;

        [Display(Name = "Seller Name")]
        public string SellerName { get; set; } = string.Empty;

        [Display(Name = "Seller Email")]
        public string SellerEmail { get; set; } = string.Empty;

        [Display(Name = "Business Name")]
        public string SellerBusinessName { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string SellerPhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Location")]
        public string SellerLocation { get; set; } = string.Empty;
    }
}