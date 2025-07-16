using ClzProject.Models;
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
        public SellerController(AppDbContext context, SignInManager<Users> signInManager, UserManager<Users> userManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult SellerEditProfile()
        {
            return View();
        }

        public IActionResult SellerDltProfile()
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

            var model = new SellerProfileViewModel
            {
                Name = user.FullName,
                Email = user.Email,
                Age = (int)user.Age,
                Location = user.Location,
                BusinessName = user.BusinessName,
                BusinessType = user.BusinessType,
                Phone = user.PhoneNumber,


            };

            return View(model);
        }

        ////AddCategory controller
        //public IActionResult AddCategory()
        //{
        //    List<SellerCategory> list = _context.SellerCategories.ToList();
        //    return View(list);
        //}
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(SellerAddProductViewModel model, IFormFile ProductImage)
        {
            if (ModelState.IsValid)
            {
                string imagePath = "";

                if (ProductImage != null && ProductImage.Length > 0)
                {
                    var fileName = Path.GetFileName(ProductImage.FileName);
                    var savePath = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await ProductImage.CopyToAsync(stream);
                    }

                    imagePath = "/images/" + fileName;
                }

                var product = new Product
                {
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    ProductPrice = model.ProductPrice,
                    ProductQuantity = model.ProductQuantity,
                    SellerCategoryCode = model.SellerCategoryCode,
                    SellerCategoryType = model.SellerCategoryType,
                    ProductImagePath = imagePath,
                    IsApproved = false // waiting for admin approval
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProductSubmitted");
            }

            return View(model);
        }

        public IActionResult A()
        {
            return View();
        }


    }
}
