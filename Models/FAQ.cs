using System.ComponentModel.DataAnnotations;

namespace ClzProject.Models
{
    public class FAQ
    {
        [Key]
        public int FAQID { get; set; }

        public string Question { get; set; } = null!;

        public string Answer { get; set; } = null!;

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
