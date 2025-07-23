using ClzProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.ViewModels
{
    [Table("Category")] // Database table name
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name = "Category Name")]

        public string CategoryType { get; set; } = null!;
        public virtual ICollection<Product>? Products { get; set; }
    }
}
