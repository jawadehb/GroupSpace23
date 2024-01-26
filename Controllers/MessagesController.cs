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
    public class MessagesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<GroupSpace23User> _userManager;

        public MessagesController(MyDbContext context, UserManager<GroupSpace23User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Message> messagesQuery;

            if (User.IsInRole("SystemAdministrator"))
            {
                // Als de gebruiker een SystemAdministrator is, toon alle berichten met Sender-informatie
                messagesQuery = _context.Messages
                    .Where(m => m.Deleted > DateTime.Now)
                    .Include(m => m.Recipient)
                    .Include(m => m.Sender);
            }
            else
            {
                // Als de gebruiker geen SystemAdministrator is, toon alleen berichten die ze hebben verstuurd zonder Sender-informatie
                messagesQuery = _context.Messages
                    .Where(m => m.Deleted > DateTime.Now && m.SenderId == currentUser.Id)
                    .Include(m => m.Recipient);
            }

            var viewModel = new MessageIndexViewModel
            {
                Messages = await messagesQuery.ToListAsync()
            };

            return View(viewModel);
        }


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

        public IActionResult Create()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;

            // If the user is a SystemAdministrator, show all groups
            // If the user is not a SystemAdministrator, show only groups they have created
            var groups = User.IsInRole("SystemAdministrator")
                ? _context.Groups.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList()
                : _context.Groups.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList();

            var groupSelectList = new SelectList(groups, "Id", "Name");

            ViewBag.RecipientId = User.IsInRole("SystemAdministrator") ? groupSelectList : new SelectList(groups.Where(g => g.StartedById == currentUser.Id), "Id", "Name");
            return View(new Message());
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,Sent,Deleted,RecipientId,SenderId,SelectedItems")] Message message)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                message.Sender = user;
                message.SenderId = user.Id;
                message.Sent = DateTime.Now;

                // Controleer op null en wijs een standaardwaarde toe als het leeg is
                message.Body = message.Body ?? "Default Body";

                // Zorg ervoor dat de geselecteerde items correct worden vastgelegd
                if (!string.IsNullOrEmpty(message.SelectedItems))
                {
                    // Voeg de geselecteerde items toe aan de Body
                    message.Body =  message.SelectedItems;
                }

                _context.Add(message);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["RecipientId"] = new SelectList(_context.Groups
                .Where(g => g.Ended > DateTime.Now)
                .OrderBy(g => g.Name), "Id", "Name", message.RecipientId);
            return View(message);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Messages == null)
            {
                return NotFound();
            }

            var message = await _context.Messages
                .Include(m => m.Recipient) // Include Recipient to access event information
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);

            // Check if the user is a SystemAdministrator or the event creator
            if (User.IsInRole("SystemAdministrator") || message.SenderId == currentUser.Id)
            {
                // If the user is a SystemAdministrator, show all events
                // If the user is the event creator, show only their own events
                var eventsToShow = User.IsInRole("SystemAdministrator")
                    ? _context.Groups.Where(g => g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList()
                    : _context.Groups.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now).OrderBy(g => g.Name).ToList();

                ViewData["RecipientId"] = new SelectList(eventsToShow, "Id", "Name", message.RecipientId);

                return View(message);
            }

            // If not a SystemAdministrator or the event creator, show an unauthorized view
            return View("Unauthorized"); // You can create a view named "Unauthorized.cshtml"
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,Sent,Deleted,RecipientId,SelectedItems")] Message updatedMessage)
        {
            if (id != updatedMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMessage = await _context.Messages
                        .Include(m => m.Sender)
                        .FirstOrDefaultAsync(m => m.Id == id);

                    if (existingMessage == null)
                    {
                        return NotFound();
                    }

                    // Update de eigenschappen van het bestaande bericht met de waarden van het updatedMessage
                    existingMessage.Title = updatedMessage.Title;
                    existingMessage.Body = updatedMessage.Body;
                    existingMessage.RecipientId = updatedMessage.RecipientId;

                    _context.Update(existingMessage);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Bewerking voltooid";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(id))
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

            ViewData["RecipientId"] = new SelectList(_context.Groups
                .Where(g => g.Ended > DateTime.Now)
                .OrderBy(g => g.Name), "Id", "Name", updatedMessage.RecipientId);

            return View(updatedMessage);
        }






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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Messages == null)
            {
                return Problem("Entity set 'GroupSpace23Context.Message' is null.");
            }

            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != message.SenderId && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return (_context.Messages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
