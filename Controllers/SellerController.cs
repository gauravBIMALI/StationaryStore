using ClzProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserRoles.Data;
using UserRoles.Models;


namespace ClzProject.Controllers
{
    public class SellerController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SellerController(AppDbContext context, SignInManager<Users> signInManager, UserManager<Users> userManager, IWebHostEnvironment env)
        {
            _env = env;
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        //This is for edit profile
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new SellerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email ?? string.Empty,
                Age = (int)user.Age,
                Location = user.Location ?? string.Empty,
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessType = user.BusinessType ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage

            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(SellerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
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
            user.Age = model.Age;
            user.Location = model.Location;
            user.BusinessName = model.BusinessName;
            user.BusinessType = model.BusinessType;
            user.PhoneNumber = model.Phone;

            // Save changes
            var result = await userManager.UpdateAsync(user);
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


        public IActionResult SellerDltProfile()
        {
            return View();
        }

        // GET: Display Profile
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new SellerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email,
                Age = (int)user.Age,
                Location = user.Location ?? string.Empty,
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessType = user.BusinessType ?? string.Empty,
                Phone = user.PhoneNumber ?? string.Empty,
                ProfileImageBase64 = user.ProfileImage // Base64 string from database
            };

            return View(model);
        }

        // Delete Profile Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DltProfile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await signInManager.SignOutAsync();

                return RedirectToAction("Login", "Account");
            }


            return RedirectToAction("EditProfile");
        }


    }
}
