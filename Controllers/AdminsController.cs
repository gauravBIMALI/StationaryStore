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

                // Fix for CS8601: Ensure null checks before assignment
                if (role == "Seller")
                {
                    userViewModel.BusinessName = user.BusinessName ?? string.Empty; // Default to empty string if null
                    userViewModel.PhoneNumber = user.PhoneNumber ?? string.Empty;   // Default to empty string if null
                }

                userViewModels.Add(userViewModel);
            }

            return View(userViewModels);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var viewModel = new AdminUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                //RegistrationDate = user.RegistrationDate,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber
            };

            if (role == "Seller")
            {
                viewModel.BusinessName = user.BusinessName;
            }

            return View(viewModel);
        }
        public IActionResult Demo()
        {
            return View();
        }
        // Add other actions (Details, Edit, Delete) as needed
    }
}
