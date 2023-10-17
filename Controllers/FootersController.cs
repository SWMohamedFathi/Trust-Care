using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrustCare.Models;

namespace TrustCare.Controllers
{
    public class FootersController : Controller
    {
        private readonly ModelContext _context;

        public FootersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Footers
        public async Task<IActionResult> Index()
        {
              return _context.Footers != null ? 
                          View(await _context.Footers.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Footers'  is null.");
        }

        // GET: Footers/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Footers == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers
                .FirstOrDefaultAsync(m => m.FooterId == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }

        // GET: Footers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Footers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FooterId,FooterLinks,CopyrightText,ContactDetails")] Footer footer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(footer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(footer);
        }

        // GET: Footers/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Footers == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers.FindAsync(id);
            if (footer == null)
            {
                return NotFound();
            }
            return View(footer);
        }

        // POST: Footers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("FooterId,FooterLinks,CopyrightText,ContactDetails")] Footer footer)
        {
            if (id != footer.FooterId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(footer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FooterExists(footer.FooterId))
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
            return View(footer);
        }

        // GET: Footers/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Footers == null)
            {
                return NotFound();
            }

            var footer = await _context.Footers
                .FirstOrDefaultAsync(m => m.FooterId == id);
            if (footer == null)
            {
                return NotFound();
            }

            return View(footer);
        }

        // POST: Footers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Footers == null)
            {
                return Problem("Entity set 'ModelContext.Footers'  is null.");
            }
            var footer = await _context.Footers.FindAsync(id);
            if (footer != null)
            {
                _context.Footers.Remove(footer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FooterExists(decimal id)
        {
          return (_context.Footers?.Any(e => e.FooterId == id)).GetValueOrDefault();
        }
    }
}
