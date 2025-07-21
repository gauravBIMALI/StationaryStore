// Models/ViewModels/AdminUserViewModel.cs
public class AdminUserViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public bool EmailConfirmed { get; set; }

    // Seller-specific properties (if applicable)
    public string BusinessName { get; set; }
    public string PhoneNumber { get; set; }

    public string UserType => Role == "Seller" ? "Seller" :
                           Role == "Admin" ? "Admin" : "Buyer";

    public string? ProfileImageBase64 { get; internal set; }
}