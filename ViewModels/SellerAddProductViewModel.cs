using System.ComponentModel.DataAnnotations;
namespace ClzProject.ViewModels
{
    public class SellerAddProductViewModel
    {
        [Required(ErrorMessage = "Product Name is required.")]
        [Display(Name = "Product Name")]

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

        [Required(ErrorMessage = "Category Type is required.")]
        [Display(Name = "Category Type")]

        public string SellerCategoryType { get; set; } = null!;


        public string SellerCategoryCode { get; set; } = null!;
        [Display(Name = "Product Image")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Please upload a valid image file (jpg, jpeg, png, gif).")]
        public IFormFile? ProductImage { get; set; }
    }

}
