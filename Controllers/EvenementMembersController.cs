using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace23.Data;
using GroupSpace23.Models;

namespace GroupSpace23.Controllers
{
    public class EvenementMembersController : Controller
    {
        private readonly MyDbContext _context;

        public EvenementMembersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: EvenementMembers
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.EvenementMembers.Include(g => g.AddedBy).Include(g => g.Evenement).Include(g => g.Member);
            return View(await myDbContext.ToListAsync());
        }

        // GET: EvenementMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EvenementMembers == null)
            {
                return NotFound();
            }

            var evenementMember = await _context.EvenementMembers
                .Include(g => g.AddedBy)
                .Include(g => g.Evenement)
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evenementMember == null)
            {
                return NotFound();
            }

            return View(evenementMember);
        }

        // GET: EvenementMembers/Create
        public IActionResult Create()
        {
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["EvenementId"] = new SelectList(_context.Evenements, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: EvenementMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EvenementId,MemberId,AddedById,Added,Removed,RemovedById,IsHost")] EvenementMember evenementMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evenementMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", evenementMember.AddedById);
            ViewData["EvenementId"] = new SelectList(_context.Evenements, "Id", "Id", evenementMember.EvenementId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", evenementMember.MemberId);
            return View(evenementMember);
        }

        // GET: EvenementMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EvenementMembers == null)
            {
                return NotFound();
            }

            var EvenementMember = await _context.EvenementMembers.FindAsync(id);
            if (EvenementMember == null)
            {
                return NotFound();
            }
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", EvenementMember.AddedById);
            ViewData["EvenementId"] = new SelectList(_context.Evenements, "Id", "Id", EvenementMember.EvenementId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", EvenementMember.MemberId);
            return View(EvenementMember);
        }

        // POST: EvenementMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EvenementId,MemberId,AddedById,Added,Removed,RemovedById,IsHost")] EvenementMember evenementMember)
        {
            if (id != evenementMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evenementMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvenementMemberExists(evenementMember.Id))
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
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", evenementMember.AddedById);
            ViewData["GvenementId"] = new SelectList(_context.Evenements, "Id", "Id", evenementMember.EvenementId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", evenementMember.MemberId);
            return View(evenementMember);
        }

        // GET: EvenementMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EvenementMembers == null)
            {
                return NotFound();
            }

            var evenementMember = await _context.EvenementMembers
                .Include(g => g.AddedBy)
                .Include(g => g.Evenement)
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evenementMember == null)
            {
                return NotFound();
            }

            return View(evenementMember);
        }

        // POST: EvenementMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.EvenementMembers == null)
            {
                return Problem("Entity set 'MyDbContext.EvenementMember'  is null.");
            }
            var evenementMember = await _context.EvenementMembers.FindAsync(id);
            if (evenementMember != null)
            {
                _context.EvenementMembers.Remove(evenementMember);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EvenementMemberExists(int id)
        {
          return (_context.EvenementMembers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
