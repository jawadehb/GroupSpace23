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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GroupSpace23.Controllers
{
    [Authorize(Roles = "SystemAdministrator,UserAdministrator,User")]
    public class MandsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<GroupSpace23User> _userManager;

        public MandsController(MyDbContext context, UserManager<GroupSpace23User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Mand> mandsQuery;

            if (User.IsInRole("SystemAdministrator"))
            {
                // Als de gebruiker een SystemAdministrator is, toon alle berichten met Sender-informatie
                mandsQuery = _context.Mands
                    .Where(m => m.Deleted > DateTime.Now)
                    .Include(m => m.Recipient)
                    .Include(m => m.Sender);
            }
            else
            {
                // Als de gebruiker geen SystemAdministrator is, toon alleen berichten die ze hebben verstuurd zonder Sender-informatie
                mandsQuery = _context.Mands
                    .Where(m => m.Deleted > DateTime.Now && m.SenderId == currentUser.Id)
                    .Include(m => m.Recipient);
            }

            var viewModel = new MandIndexViewModel
            {
                Mands = await mandsQuery.ToListAsync()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Mands == null)
            {
                return NotFound();
            }

            var mand = await _context.Mands
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mand == null)
            {
                return NotFound();
            }

            return View(mand);
        }

        public IActionResult Create()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            // If the user is a SystemAdministrator, show all evenements
            // If the user is not a SystemAdministrator, show only evenements they have created
            var evenements = User.IsInRole("SystemAdministrator")
                ? _context.Evenements.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList()
                : _context.Evenements.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList();

            var evenementSelectList = new SelectList(evenements, "Id", "Name");

            ViewBag.RecipientId = User.IsInRole("SystemAdministrator") ? evenementSelectList : new SelectList(evenements.Where(g => g.StartedById == currentUser.Id), "Id", "Name");
            return View(new Mand());
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Sent,Deleted,RecipientId,SenderId,SelectedItems")] Mand mand)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                mand.Sender = user;
                mand.SenderId = user.Id;
                mand.Sent = DateTime.Now;

                // Controleer op null en wijs een standaardwaarde toe als het leeg is
                mand.Body = mand.Body ?? "Default Body";

                // Zorg ervoor dat de geselecteerde items correct worden vastgelegd
                if (!string.IsNullOrEmpty(mand.SelectedItems))
                {
                    // Voeg de geselecteerde items toe aan de Body
                    mand.Body =  mand.SelectedItems;
                }

                _context.Add(mand);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["RecipientId"] = new SelectList(_context.Evenements
                .Where(g => g.Ended > DateTime.Now)
                .OrderBy(g => g.Name), "Id", "Name", mand.RecipientId);
            return View(mand);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Mands == null)
            {
                return NotFound();
            }

            var mand = await _context.Mands
                .Include(m => m.Recipient) // Include Recipient to access event information
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mand == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the user is a SystemAdministrator or the event creator
            if (User.IsInRole("SystemAdministrator") || mand.SenderId == currentUser.Id)
            {
                // If the user is a SystemAdministrator, show all events
                // If the user is the event creator, show only their own events
                var eventsToShow = User.IsInRole("SystemAdministrator")
                    ? _context.Evenements.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList()
                    : _context.Evenements.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList();

                ViewData["RecipientId"] = new SelectList(eventsToShow, "Id", "Name", mand.RecipientId);

                return View(mand);
            }

            // If not a SystemAdministrator or the event creator, show an unauthorized view
            return View("Unauthorized"); // You can create a view named "Unauthorized.cshtml"
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Sent,Deleted,RecipientId,SelectedItems")] Mand updatedMand)
        {
            if (id != updatedMand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMand = await _context.Mands
                        .Include(m => m.Sender)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingMand == null)
                    {
                        return NotFound();
                    }

                    // Update de eigenschappen van het bestaande bericht met de waarden van het updatedMand
                    existingMand.Title = updatedMand.Title;
                    existingMand.Body = updatedMand.Body;
                    existingMand.RecipientId = updatedMand.RecipientId;

                    _context.Update(existingMand);
                    await _context.SaveChangesAsync();

                    TempData["Mand"] = "Bewerking voltooid";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MandExists(id))
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

            ViewData["RecipientId"] = new SelectList(_context.Evenements
                .Where(g => g.Ended > DateTime.Now)
                .OrderBy(g => g.Name), "Id", "Name", updatedMand.RecipientId);

            return View(updatedMand);
        }






        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Mands == null)
            {
                return NotFound();
            }

            var mand = await _context.Mands
                .Include(m => m.Recipient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mand == null)
            {
                return NotFound();
            }

            return View(mand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Mands == null)
            {
                return Problem("Entity set 'GroupSpace23Context.Mand' is null.");
            }

            var mand = await _context.Mands.FindAsync(id);

            if (mand == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != mand.SenderId && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            _context.Mands.Remove(mand);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MandExists(int id)
        {
            return (_context.Mands?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
