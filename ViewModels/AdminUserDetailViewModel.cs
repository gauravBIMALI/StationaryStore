namespace ClzProject.ViewModels
{
    public class AdminUserDetailViewModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = null!;

        public string? ProfileImageBase64 { get; set; }

        // Optional fields (only populated for sellers)
        public int? Age { get; set; }
        public string Location { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public string BusinessType { get; set; } = null!;

        public DateTime RegisteredDate { get; set; }
    }

}
