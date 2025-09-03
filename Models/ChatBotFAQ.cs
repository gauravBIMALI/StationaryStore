namespace ClzProject.Models
{
    public class ChatBotFAQ
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Role { get; set; } = "All"; // Buyer, Seller, Admin, All
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
