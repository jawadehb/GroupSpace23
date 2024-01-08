using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace23.Data;
using GroupSpace23.Models;
using GroupSpace23.Areas.Identity.Data;

namespace GroupSpace23.Controllers
{
    public class MessagesController : Controller
    {
        private readonly MyDbContext _context;

        public MessagesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Messages
        public async Task<IActionResult> Index(string selectMode = "R")
        {
            List<ModeItem> modeItems = new List<ModeItem>
                {
                    new ModeItem {Value = "R", Text = "Ontvangen"},
                    new ModeItem {Value = "S", Text = "Verzonden"}
                };

            MessageIndexViewModel viewModel = new MessageIndexViewModel();
            viewModel.SelectMode = selectMode;
            viewModel.Modes = new SelectList(modeItems, "Value", "Text", selectMode);

            if (selectMode == "R")
            {
                viewModel.Messages = _context.Messages
                                                .Where(m => m.Deleted > DateTime.Now)
                                                .Include(m => m.Recipient)
                                                .Include(m => m.Sender)
                                                .ToList();
            }
            else
            {
                GroupSpace23User user = _context.Users.First(u => u.UserName == User.Identity.Name);
                viewModel.Messages = _context.Messages
                                   .Where(m => m.Deleted > DateTime.Now && m.SenderId == user.Id)
                                   .Include(m => m.Recipient)
                                   .Include(m => m.Sender)
                                   .ToList();
            }
            return View(viewModel);
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            ViewData["RecipientId"] = new SelectList(_context.Groups
                                        .Where(g => g.Ended > DateTime.Now)
                                        .OrderBy(g => g.Name), "Id", "Name");
            //            ViewBag.RecipientId = new SelectList(_context.Groups.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name), "Id", "Name");
            return View(new Message());
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Sent,Deleted,RecipientId,SenderId")] Message message)
        {
            if (ModelState.IsValid)
            {
                message.Sender = _context.Users.First(u => u.UserName == User.Identity.Name);
                message.SenderId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                message.Sent = DateTime.Now;
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RecipientId"] = new SelectList(_context.Groups
                                        .Where(g => g.Ended > DateTime.Now)
                                        .OrderBy(g => g.Name), "Id", "Name", message.RecipientId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["RecipientId"] = new SelectList(_context.Groups.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name), "Id", "Name", message.RecipientId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Sent,Deleted,RecipientId")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["RecipientId"] = new SelectList(_context.Groups.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name), "Id", "Name", message.RecipientId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Messages == null)
            {
                return Problem("Entity set 'GroupSpace23Context.Message'  is null.");
            }
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return (_context.Messages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}