using ClzProject.Models;
using ClzProject.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    public class AddProductController : Controller
    {
        private readonly AppDbContext _context;

        public AddProductController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AddProduct/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryType
                }).ToListAsync();

            var model = new ProductViewModel
            {
                CategoryList = categories
            };

            return View(model);
        }

        // POST: AddProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ProductImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await model.ProductImageFile.CopyToAsync(ms);
                    model.ProductImage = Convert.ToBase64String(ms.ToArray());
                }

                var product = new Product
                {
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    ProductImage = model.ProductImage,
                    CategoryId = model.CategoryId
                };

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If model invalid, re-populate dropdown
            model.CategoryList = await _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryType
                }).ToListAsync();

            return View(model);
        }

        // GET: AddProduct/Index
        public async Task<IActionResult> Index()
        {
            var products = await _context.Product
                .Include(p => p.Category)
                .ToListAsync();

            return View(products);
        }
    }
}
