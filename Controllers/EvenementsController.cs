// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GroupSpace23.Data;
using GroupSpace23.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using GroupSpace23.Areas.Identity.Data;

namespace GroupSpace23.Controllers
{
    [Authorize]
    public class EvenementsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<GroupSpace23User> _userManager;

        public EvenementsController(MyDbContext context, UserManager<GroupSpace23User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Evenements
        [AllowAnonymous]
        public async Task<IActionResult> Index(DateTime? filterDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Evenement> evenementsQuery;

            if (User.IsInRole("SystemAdministrator"))
            {
                // Als de gebruiker een SystemAdministrator is, toon alle groepen
                evenementsQuery = _context.Evenements.Where(g => g.Ended > DateTime.Now);
            }
            else
            {
                // Als de gebruiker geen SystemAdministrator is, toon alleen de groepen die ze hebben gemaakt
                evenementsQuery = _context.Evenements.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now);
            }

            if (filterDate.HasValue)
            {
                evenementsQuery = evenementsQuery.Where(g => g.Started.Date == filterDate.Value.Date);
            }

            List<Evenement> evenements = await evenementsQuery.ToListAsync();

            ViewBag.FilterDate = filterDate.HasValue ? filterDate.Value.ToString("yyyy-MM-dd") : null;

            return View(evenements);
        }





        // GET: Evenements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Evenements == null)
            {
                return NotFound();
            }

            var @evenement = await _context.Evenements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@evenement == null)
            {
                return NotFound();
            }

            return View(@evenement);
        }

        // GET: Evenements/Create
        public IActionResult Create()
        {
            return View(new Evenement());
        }

        // POST: Evenements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Started,Ended,StartedById")] Evenement @evenement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@evenement);
                evenement.StartedById = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@evenement);
        }

        // GET: Evenements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Evenements == null)
            {
                return NotFound();
            }

            var @evenement = await _context.Evenements.FindAsync(id);
            if (@evenement == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the evenement or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @evenement.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            return View(@evenement);
        }

        // POST: Evenements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Started,Ended,StartedById")] Evenement @evenement)
        {
            if (id != @evenement.Id)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the evenement or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @evenement.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@evenement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvenementExists(@evenement.Id))
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
            return View(@evenement);
        }


        // GET: Evenements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Evenements == null)
            {
                return NotFound();
            }

            var @evenement = await _context.Evenements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@evenement == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the evenement or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @evenement.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            return View(@evenement);
        }

        // POST: Evenements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Evenements == null)
            {
                return Problem("Entity set 'GroupSpace23Context.Evenement' is null.");
            }

            var evenement = await _context.Evenements.FindAsync(id);

            if (evenement == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the evenement or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != evenement.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            evenement.Ended = DateTime.Now;
            _context.Evenements.Update(evenement);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EvenementExists(int id)
        {
            return (_context.Evenements?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

