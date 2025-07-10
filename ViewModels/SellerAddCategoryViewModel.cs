using System.ComponentModel.DataAnnotations;

namespace ClzProject.ViewModels
{
    public class SellerAddCategoryViewModel
    {
        [Key]
        public int SellerCategoryId { get; set; }

        [Required(ErrorMessage = "Category code is required.")]
        [Display(Name = "Category Code")]
        public string SellerCategoryCode { get; set; } = null!;

        [Required(ErrorMessage = "Category type is required.")]
        [Display(Name = "Category Type")]
        public string SellerCategoryType { get; set; } = null!;
    }
}
