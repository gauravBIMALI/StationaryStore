
using ClzProject.Models;
using ClzProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminsController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;

        public AdminsController(UserManager<Users> userManager,
                             RoleManager<IdentityRole> roleManager,
                             AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]

        //This is for admin profile create
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminProfileViewModel
            {
                Name = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage // Base64 string from database
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        //This is for edit profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminProfileViewModel
            {
                Name = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage
            };

            return View(model);
        }

        // POST: Update Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(AdminProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Handle image upload
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                // Validate image
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(model.ProfileImage.ContentType))
                {
                    ModelState.AddModelError("ProfileImage", "Only JPG, PNG or GIF images are allowed.");
                    return View(model);
                }

                // Limit file size to 2MB
                if (model.ProfileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfileImage", "Image must be smaller than 2MB.");
                    return View(model);
                }

                // Convert to Base64
                using (var memoryStream = new MemoryStream())
                {
                    await model.ProfileImage.CopyToAsync(memoryStream);
                    user.ProfileImage = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            // Update other profile fields
            user.FullName = model.Name;
            user.Email = model.Email;
            user.UserName = model.Email;

            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            // Handle errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "User";

                var userViewModel = new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Role = role,
                    //RegistrationDate = user.RegistrationDate,
                    EmailConfirmed = user.EmailConfirmed
                };


                if (role == "Seller")
                {
                    userViewModel.BusinessName = user.BusinessName ?? string.Empty; // Default to empty string if null
                    userViewModel.PhoneNumber = user.PhoneNumber ?? string.Empty;   // Default to empty string if null
                }

                userViewModels.Add(userViewModel);
            }

            return View(userViewModels);
        }

        //this Code is for manageUser Action buttons
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UserDetail(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var viewModel = new AdminUserDetailViewModel
            {
                Name = user.FullName,
                Email = user.Email,
                Role = role,
                ProfileImageBase64 = user.ProfileImage,

                Age = user.Age,
                Location = user.Location ?? string.Empty, // Default to empty string if null
                Phone = user.PhoneNumber ?? string.Empty,
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessType = user.BusinessType ?? string.Empty
            };

            if (role == "Seller")
            {
                viewModel.Phone = user.PhoneNumber ?? string.Empty;
                viewModel.BusinessName = user.BusinessName ?? string.Empty;
                viewModel.BusinessType = user.BusinessType ?? string.Empty;
                viewModel.Location = user.Location ?? string.Empty;
                viewModel.Age = user.Age;
            }

            return View("UserDetail", viewModel);
        }
        [Authorize(Roles = "Admin")]
        //this code is for Disabling user by admin
        public IActionResult UserDisable()
        {
            return View();
        }
        //FOR DISPLAYING SELLER PRODUCTS
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListProducts()
        {
            // Load products WITHOUT images for faster initial load
            var products = await _context.Product
                .Select(p => new Product
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    ProductDescription = p.ProductDescription,
                    ProductPrice = p.ProductPrice,
                    ProductQuantity = p.ProductQuantity,
                    CategoryType = p.CategoryType,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    SellerId = p.SellerId
                    // Deliberately NOT selecting Image field
                })
                .ToListAsync();

            // Create a list to hold product with seller info
            var productList = new List<ProductWithSellerViewModel>();

            foreach (var product in products)
            {
                // Get seller information using your Users model
                var seller = await _userManager.FindByIdAsync(product.SellerId);
                productList.Add(new ProductWithSellerViewModel
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    ProductPrice = product.ProductPrice,
                    ProductQuantity = product.ProductQuantity,
                    CategoryType = product.CategoryType,
                    // Image will be loaded separately via AJAX
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    SellerId = product.SellerId,
                    SellerName = seller?.FullName ?? "Unknown Seller",
                    SellerEmail = seller?.Email ?? "No Email",
                    SellerBusinessName = seller?.BusinessName ?? "No Business Name"
                });
            }

            return View(productList);
        }

        // Add this new method for loading images separately (for admin view)
        // Add this method to your AdminsController
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProductImageAdmin(int id)
        {
            var image = await _context.Product
                .Where(p => p.ProductID == id)
                .Select(p => p.Image)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(image))
                return NotFound();

            return Json(new { imageBase64 = image });
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            // Get seller information
            var seller = await _userManager.FindByIdAsync(product.SellerId);

            var viewModel = new ProductDetailsViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantity = product.ProductQuantity,
                CategoryType = product.CategoryType,
                Image = product.Image,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                SellerId = product.SellerId,
                SellerName = seller?.FullName ?? "Unknown Seller",
                SellerEmail = seller?.Email ?? "No Email",
                SellerBusinessName = seller?.BusinessName ?? "No Business Name",
                SellerPhoneNumber = seller?.PhoneNumber ?? "No Phone",
                SellerLocation = seller?.Location ?? "No Location"
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            // Get seller information
            var seller = await _userManager.FindByIdAsync(product.SellerId);

            var viewModel = new ProductDeletionViewModel
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                ProductQuantity = product.ProductQuantity,
                CategoryType = product.CategoryType,
                Image = product.Image,
                CreatedAt = product.CreatedAt,
                SellerId = product.SellerId,
                SellerName = seller?.FullName ?? "Unknown Seller",
                SellerBusinessName = seller?.BusinessName ?? "No Business Name",
                // For admin to enter deletion reason
                DeletionReason = string.Empty
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductConfirmed(ProductDeletionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DeleteProduct", model);
            }

            var product = await _context.Product.FindAsync(model.ProductID);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found!";
                return RedirectToAction(nameof(ListProducts));
            }

            // Get current admin info
            var currentAdmin = await _userManager.GetUserAsync(User);

            // Create notification for seller
            var notification = new ProductDeletionNotification
            {
                SellerId = product.SellerId,
                ProductName = product.ProductName,
                DeletionReason = model.DeletionReason,
                AdminName = currentAdmin?.FullName ?? "Admin",
                DeletedAt = DateTime.Now,
                IsRead = false
            };

            // Save notification
            _context.ProductDeletionNotifications.Add(notification);

            // Delete the product
            _context.Product.Remove(product);

            // Save all changes
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Product '{product.ProductName}' has been deleted and seller has been notified.";

            return RedirectToAction(nameof(ListProducts));
        }

        //for admin dashboard
        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Get user counts
            var sellers = await _userManager.GetUsersInRoleAsync("Seller");
            var users = await _userManager.GetUsersInRoleAsync("User");

            var dashboardData = new AdminDashboardViewModel
            {
                // Total number of sellers
                TotalSellers = sellers.Count,

                // Total number of listed products
                TotalProducts = await _context.Product.CountAsync(),

                // Total categories (adjust table name if different)
                TotalCategories = await _context.Category.CountAsync(),

                // Total FAQs
                TotalFAQs = await _context.FAQs.CountAsync(),

                // Additional useful metrics
                TotalUsers = users.Count,

                // Recent activities (optional)
                RecentProducts = await _context.Product
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new { p.ProductName, p.CreatedAt, p.CategoryType })
                    .ToListAsync(),

                // Recent sellers
                RecentSellers = sellers.OrderByDescending(s => s.Id).Take(5).ToList()
            };

            return View(dashboardData);
        }


    }
}
