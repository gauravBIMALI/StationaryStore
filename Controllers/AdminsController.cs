//using ClzProject.ViewModels;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using UserRoles.Data;
//using UserRoles.Models;

//namespace ClzProject.Controllers
//{
//    //[Authorize(Roles = "Admin")]
//    //[Area("Admin")]
//    public class AdminsController : Controller
//    {
//        private readonly UserManager<Users> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly AppDbContext _context;

//        public AdminsController(UserManager<Users> userManager,
//                             RoleManager<IdentityRole> roleManager,
//                             AppDbContext context)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _context = context;
//        }


//        //This is for admin profile create
//        public async Task<IActionResult> Profile()
//        {
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return NotFound();
//            }

//            var model = new AdminProfileViewModel
//            {
//                Name = user.FullName,
//                Email = user.Email
//            };

//            return View(model);
//        }

//        //This is for edit profile
//        [HttpGet]
//        public async Task<IActionResult> EditProfile()
//        {
//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return NotFound();
//            }

//            var model = new AdminProfileViewModel
//            {
//                Name = user.FullName,
//                Email = user.Email
//            };

//            return View(model);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> EditProfile(AdminProfileViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return View(model);
//            }

//            var user = await _userManager.GetUserAsync(User);

//            if (user == null)
//            {
//                return NotFound();
//            }

//            // Update the fields
//            user.FullName = model.Name;
//            user.Email = model.Email;
//            user.UserName = model.Email;


//            var result = await _userManager.UpdateAsync(user);

//            if (result.Succeeded)
//            {
//                TempData["Success"] = "Profile updated successfully!";
//                return RedirectToAction("Profile");
//            }

//            // If update fails, add errors to ModelState
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError(string.Empty, error.Description);
//            }

//            return View(model);
//        }



//        //this is for dlt profile
//        public IActionResult DltProfile()
//        {
//            return View();
//        }

//        public async Task<IActionResult> Index()
//        {
//            var users = await _userManager.Users.ToListAsync();
//            var userViewModels = new List<AdminUserViewModel>();

//            foreach (var user in users)
//            {
//                var roles = await _userManager.GetRolesAsync(user);
//                var role = roles.FirstOrDefault() ?? "User";

//                var userViewModel = new AdminUserViewModel
//                {
//                    Id = user.Id,
//                    FullName = user.FullName,
//                    UserName = user.UserName,
//                    Email = user.Email,
//                    Role = role,
//                    //RegistrationDate = user.RegistrationDate,
//                    EmailConfirmed = user.EmailConfirmed
//                };


//                if (role == "Seller")
//                {
//                    userViewModel.BusinessName = user.BusinessName ?? string.Empty; // Default to empty string if null
//                    userViewModel.PhoneNumber = user.PhoneNumber ?? string.Empty;   // Default to empty string if null
//                }

//                userViewModels.Add(userViewModel);
//            }

//            return View(userViewModels);
//        }

//    }
//}

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
                Name = user.FullName,
                Email = user.Email,
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
                Name = user.FullName,
                Email = user.Email,
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



        //this is for dlt profile
        public IActionResult DltProfile()
        {
            return View();
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
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
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

    }
}
