using ClzProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    public class BuyerController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;

        public BuyerController(SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Display Profile
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new BuyerProfileViewModel
            {
                Name = user.FullName,

                Email = user.Email,
                ProfileImageBase64 = user.ProfileImage // Base64 string from database
            };

            return View(model);
        }

        // GET: Edit Profile Form
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new BuyerProfileViewModel
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
        public async Task<IActionResult> EditProfile(BuyerProfileViewModel model)
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

        // Delete Profile Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DltProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();

                return RedirectToAction("Login", "Account");
            }


            return RedirectToAction("EditProfile");
        }




    }
}