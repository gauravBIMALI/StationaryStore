using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.ViewModels
{
    [Table("Product")]
    public class ProductViewModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Description is required.")]
        [Display(Name = "Description")]
        public string ProductDescription { get; set; } = null!;

        [Display(Name = "Product Image")]
        public string? ProductImage { get; set; } // Stored as base64

        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ProductImageFile { get; set; } // For file input

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
    }
}
