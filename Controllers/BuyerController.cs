using ClzProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserRoles.Data;
using UserRoles.Models;

namespace ClzProject.Controllers
{
    public class BuyerController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly AppDbContext _context;
        public BuyerController(AppDbContext context, SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }


        //this is for buyer edit profile
        public IActionResult EditProfile()
        {
            return View();
        }


        //this is for buyer dlt profile
        public IActionResult DltProfile()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var model = new BuyerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email,
            };

            return View(model);
        }
    }
}
