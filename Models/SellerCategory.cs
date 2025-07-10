using System.ComponentModel.DataAnnotations;

namespace ClzProject.Models
{
    public class SellerCategory
    {
        [Key]
        public int SellerCategoryId { get; set; }
        public string SellerCategoryCode { get; set; } = null!;
        public string SellerCategoryType { get; set; } = null!;
    }
}
