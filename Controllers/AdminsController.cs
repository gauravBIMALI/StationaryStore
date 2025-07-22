
using ClzProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    //[Authorize(Roles = "Admin")]
    //[Area("Admin")]
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
        //this code is for Disabling user by admin
        public IActionResult UserDisable()
        {
            return View();
        }


    }
}
