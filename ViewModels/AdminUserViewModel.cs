// Models/ViewModels/AdminUserViewModel.cs
public class AdminUserViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    //public DateTime? RegistrationDate { get; set; }
    public bool EmailConfirmed { get; set; }

    // Seller-specific properties (if applicable)
    public string BusinessName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;

    public string UserType => Role == "Seller" ? "Seller" :
                           Role == "Admin" ? "Admin" : "Buyer";

    public string? ProfileImageBase64 { get; set; }
}