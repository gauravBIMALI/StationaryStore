using System.ComponentModel.DataAnnotations;

namespace ClzProject.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = null!;
        [Required(ErrorMessage = "Product Description is required.")]
        [Display(Name = "Product Description")]
        [StringLength(550, ErrorMessage = "Description cannot exceed 550 characters.")]
        public string ProductDescription { get; set; } = null!;
        [Required(ErrorMessage = "Product Price is required.")]
        [Display(Name = "Product Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]

        public decimal ProductPrice { get; set; }
        [Required(ErrorMessage = "Product Quantity is required.")]
        [Display(Name = "Product Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int ProductQuantity { get; set; }
        [Display(Name = "Product Image")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Please upload a valid image file (jpg, jpeg, png, gif).")]
        public string? ProductImagePath { get; set; }
        [Required(ErrorMessage = "Category Type is required.")]
        [Display(Name = "Category Type")]
        [StringLength(50, ErrorMessage = "Category type cannot exceed 50 characters.")]
        public string SellerCategoryType { get; set; } = null!;
        [Required(ErrorMessage = "Category code is required.")]
        [Display(Name = "Category Code")]
        [StringLength(50, ErrorMessage = "Category code cannot exceed 50 characters.")]
        public string SellerCategoryCode { get; set; } = null!;
        [Display(Name = "Is Approved")]
        [Required(ErrorMessage = "Approval status is required.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Product must be approved.")]
        public bool IsApproved { get; set; } = false;
    }

}
