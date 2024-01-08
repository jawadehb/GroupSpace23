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
    public class GroupMembersController : Controller
    {
        private readonly MyDbContext _context;

        public GroupMembersController(MyDbContext context)
        {
            _context = context;
        }

        // GET: GroupMembers
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.GroupMembers.Include(g => g.AddedBy).Include(g => g.Group).Include(g => g.Member);
            return View(await myDbContext.ToListAsync());
        }

        // GET: GroupMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.GroupMembers == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers
                .Include(g => g.AddedBy)
                .Include(g => g.Group)
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember);
        }

        // GET: GroupMembers/Create
        public IActionResult Create()
        {
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id");
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: GroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupId,MemberId,AddedById,Added,Removed,RemovedById,IsHost")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", groupMember.AddedById);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupMember.GroupId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", groupMember.MemberId);
            return View(groupMember);
        }

        // GET: GroupMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.GroupMembers == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers.FindAsync(id);
            if (groupMember == null)
            {
                return NotFound();
            }
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", groupMember.AddedById);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupMember.GroupId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", groupMember.MemberId);
            return View(groupMember);
        }

        // POST: GroupMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupId,MemberId,AddedById,Added,Removed,RemovedById,IsHost")] GroupMember groupMember)
        {
            if (id != groupMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupMemberExists(groupMember.Id))
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
            ViewData["AddedById"] = new SelectList(_context.Users, "Id", "Id", groupMember.AddedById);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupMember.GroupId);
            ViewData["MemberId"] = new SelectList(_context.Users, "Id", "Id", groupMember.MemberId);
            return View(groupMember);
        }

        // GET: GroupMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.GroupMembers == null)
            {
                return NotFound();
            }

            var groupMember = await _context.GroupMembers
                .Include(g => g.AddedBy)
                .Include(g => g.Group)
                .Include(g => g.Member)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupMember == null)
            {
                return NotFound();
            }

            return View(groupMember);
        }

        // POST: GroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.GroupMembers == null)
            {
                return Problem("Entity set 'MyDbContext.GroupMember'  is null.");
            }
            var groupMember = await _context.GroupMembers.FindAsync(id);
            if (groupMember != null)
            {
                _context.GroupMembers.Remove(groupMember);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupMemberExists(int id)
        {
          return (_context.GroupMembers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
