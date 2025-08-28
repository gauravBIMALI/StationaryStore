using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClzProject.Models
{
    [Table("Comments")] // DB Table Name
    public class Comments
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        public string Comment { get; set; } = null!;

        //[ForeignKey("FullName")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = null!;
    }
}
