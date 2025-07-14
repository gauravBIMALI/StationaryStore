using ClzProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    public class SellerAddCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public SellerAddCategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: SellerAddCategory
        public async Task<IActionResult> Index()
        {

            return View(await _context.SellerAddCategoryViewModel.ToListAsync());
        }

        // GET: SellerAddCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerAddCategoryViewModel = await _context.SellerAddCategoryViewModel
                .FirstOrDefaultAsync(m => m.SellerCategoryId == id);
            if (sellerAddCategoryViewModel == null)
            {
                return NotFound();
            }

            return View(sellerAddCategoryViewModel);
        }

        // GET: SellerAddCategory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SellerAddCategory/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SellerCategoryId,SellerCategoryCode,SellerCategoryType")] SellerAddCategoryViewModel sellerAddCategoryViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sellerAddCategoryViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sellerAddCategoryViewModel);
        }

        // GET: SellerAddCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerAddCategoryViewModel = await _context.SellerAddCategoryViewModel.FindAsync(id);
            if (sellerAddCategoryViewModel == null)
            {
                return NotFound();
            }
            return View(sellerAddCategoryViewModel);
        }

        // POST: SellerAddCategory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SellerCategoryId,SellerCategoryCode,SellerCategoryType")] SellerAddCategoryViewModel sellerAddCategoryViewModel)
        {
            if (id != sellerAddCategoryViewModel.SellerCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sellerAddCategoryViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SellerAddCategoryViewModelExists(sellerAddCategoryViewModel.SellerCategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sellerAddCategoryViewModel);
        }

        // GET: SellerAddCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sellerAddCategoryViewModel = await _context.SellerAddCategoryViewModel
                .FirstOrDefaultAsync(m => m.SellerCategoryId == id);
            if (sellerAddCategoryViewModel == null)
            {
                return NotFound();
            }

            return View(sellerAddCategoryViewModel);
        }

        // POST: SellerAddCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sellerAddCategoryViewModel = await _context.SellerAddCategoryViewModel.FindAsync(id);
            if (sellerAddCategoryViewModel != null)
            {
                _context.SellerAddCategoryViewModel.Remove(sellerAddCategoryViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SellerAddCategoryViewModelExists(int id)
        {
            return _context.SellerAddCategoryViewModel.Any(e => e.SellerCategoryId == id);
        }
    }
}
