using ClzProject.Models;
using Microsoft.AspNetCore.Mvc;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    public class ChatBotFAQController : Controller
    {
        private readonly AppDbContext _context;

        public ChatBotFAQController(AppDbContext context)
        {
            _context = context;
        }

        // GET: List all FAQs
        public IActionResult Index()
        {
            var faqs = _context.ChatBotFAQs.ToList();
            return View(faqs);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChatBotFAQ faq)
        {
            if (ModelState.IsValid)
            {
                _context.ChatBotFAQs.Add(faq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faq);
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var faq = _context.ChatBotFAQs.Find(id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ChatBotFAQ faq)
        {
            if (ModelState.IsValid)
            {
                _context.ChatBotFAQs.Update(faq);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faq);
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var faq = _context.ChatBotFAQs.Find(id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faq = _context.ChatBotFAQs.Find(id);
            if (faq != null)
            {
                _context.ChatBotFAQs.Remove(faq);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
