using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ClzProject.ViewModels
{
    public class SellerAddProductViewModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters.")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Product description is required.")]
        [Display(Name = "Product Description")]
        [StringLength(500, ErrorMessage = "Product description cannot exceed 500 characters.")]
        public string ProductDescription { get; set; } = null!;

        [Required(ErrorMessage = "Product price is required.")]
        [Display(Name = "Product Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Product price must be greater than 0.")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Product Image")]
        public string? ProductImage { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int SellerCategoryId { get; set; }

        // Navigation property to access category details
        [ForeignKey("SellerCategoryId")]
        public virtual SellerAddCategoryViewModel? Category { get; set; }

        // For dropdown in create view
        [NotMapped]
        public SelectList? CategoryList { get; set; }
    }

}
