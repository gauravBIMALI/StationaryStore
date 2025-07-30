using ClzProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    public class AdminContactsController : Controller
    {
        private readonly AppDbContext _context;

        public AdminContactsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AdminContacts
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminContact.ToListAsync());
        }

        // GET: AdminContacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (adminContact == null)
            {
                return NotFound();
            }

            return View(adminContact);
        }

        // GET: AdminContacts/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContactId,Name,Email,Age,Location,BusinessName,BusinessType,Phone,PANBase64,VerifiedIDBase64")] AdminContact adminContact)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminContact);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminContact);
        }

        // GET: AdminContacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact.FindAsync(id);
            if (adminContact == null)
            {
                return NotFound();
            }
            return View(adminContact);
        }

        // POST: AdminContacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContactId,Name,Email,Age,Location,BusinessName,BusinessType,Phone,PANBase64,VerifiedIDBase64")] AdminContact adminContact)
        {
            if (id != adminContact.ContactId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminContact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminContactExists(adminContact.ContactId))
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
            return View(adminContact);
        }

        // GET: AdminContacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminContact = await _context.AdminContact
                .FirstOrDefaultAsync(m => m.ContactId == id);
            if (adminContact == null)
            {
                return NotFound();
            }

            return View(adminContact);
        }

        // POST: AdminContacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminContact = await _context.AdminContact.FindAsync(id);
            if (adminContact != null)
            {
                _context.AdminContact.Remove(adminContact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminContactExists(int id)
        {
            return _context.AdminContact.Any(e => e.ContactId == id);
        }



    }
}
