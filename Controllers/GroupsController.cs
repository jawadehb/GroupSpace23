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
    public class GroupsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<GroupSpace23User> _userManager;

        public GroupsController(MyDbContext context, UserManager<GroupSpace23User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Groups
        [AllowAnonymous]
        public async Task<IActionResult> Index(DateTime? filterDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            IQueryable<Group> groupsQuery;

            if (User.IsInRole("SystemAdministrator"))
            {
                // Als de gebruiker een SystemAdministrator is, toon alle groepen
                groupsQuery = _context.Groups.Where(g => g.Ended > DateTime.Now);
            }
            else
            {
                // Als de gebruiker geen SystemAdministrator is, toon alleen de groepen die ze hebben gemaakt
                groupsQuery = _context.Groups.Where(g => g.StartedById == currentUser.Id && g.Ended > DateTime.Now);
            }

            if (filterDate.HasValue)
            {
                groupsQuery = groupsQuery.Where(g => g.Started.Date == filterDate.Value.Date);
            }

            List<Group> groups = await groupsQuery.ToListAsync();

            ViewBag.FilterDate = filterDate.HasValue ? filterDate.Value.ToString("yyyy-MM-dd") : null;

            return View(groups);
        }





        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View(new Group());
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Started,Ended,StartedById")] Group @group)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@group);
                group.StartedById = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the group or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @group.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            return View(@group);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Started,Ended,StartedById")] Group @group)
        {
            if (id != @group.Id)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the group or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @group.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.Id))
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
            return View(@group);
        }


        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Groups == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@group == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the group or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != @group.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Groups == null)
            {
                return Problem("Entity set 'GroupSpace23Context.Group' is null.");
            }

            var group = await _context.Groups.FindAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the group or an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser.Id != group.StartedById && !User.IsInRole("SystemAdministrator"))
            {
                return Forbid();
            }

            group.Ended = DateTime.Now;
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return (_context.Groups?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

