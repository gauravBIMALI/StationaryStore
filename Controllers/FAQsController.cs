using ClzProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;

namespace ClzProject.Controllers
{

    public class FAQsController : Controller
    {
        private readonly AppDbContext _context;

        public FAQsController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize(Roles = "Admin")]
        // GET: FAQs
        public async Task<IActionResult> Index()
        {
            return View(await _context.FAQs.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: FAQs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQ = await _context.FAQs
                .FirstOrDefaultAsync(m => m.FAQID == id);
            if (fAQ == null)
            {
                return NotFound();
            }

            return View(fAQ);
        }

        [Authorize(Roles = "Admin")]
        // GET: FAQs/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FAQID,Question,Answer,CreatedDate,ModifiedDate")] FAQ fAQ)
        {
            if (ModelState.IsValid)
            {
                fAQ.CreatedDate = DateTime.UtcNow;
                _context.Add(fAQ);
                await _context.SaveChangesAsync();

                TempData["ToastMessage"] = "FAQ created successfully!";
                TempData["ToastType"] = "success";
                return RedirectToAction(nameof(Index));
            }

            TempData["ToastMessage"] = "Failed to create FAQ. Please check the form.";
            TempData["ToastType"] = "error";
            return View(fAQ);
        }

        [Authorize(Roles = "Admin")]
        // GET: FAQs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQ = await _context.FAQs.FindAsync(id);
            if (fAQ == null)
            {
                return NotFound();
            }
            return View(fAQ);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FAQID,Question,Answer,CreatedDate,ModifiedDate")] FAQ fAQ)
        {
            if (id != fAQ.FAQID)
            {

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(fAQ);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FAQExists(fAQ.FAQID))
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
            return View(fAQ);
        }
        [Authorize(Roles = "Admin")]
        // GET: FAQs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fAQ = await _context.FAQs
                .FirstOrDefaultAsync(m => m.FAQID == id);
            if (fAQ == null)
            {
                return NotFound();
            }

            return View(fAQ);
        }

        // POST: FAQs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fAQ = await _context.FAQs.FindAsync(id);
            if (fAQ != null)
            {
                _context.FAQs.Remove(fAQ);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FAQExists(int id)
        {
            return _context.FAQs.Any(e => e.FAQID == id);
        }


        //THIS IS FOR SELLER
        [Authorize(Roles = "Seller")]
        public IActionResult SellerFAQ()
        {
            // This action can be used to display FAQs specifically for sellers
            var faqs = _context.FAQs.ToList();
            return View(faqs);
        }
        //[Authorize(Roles = "Buyer")]
        //For Buyer
        public async Task<IActionResult> BuyerFAQ()
        {
            var faqs = await _context.FAQs.ToListAsync();
            return View(faqs);
        }

    }
}
