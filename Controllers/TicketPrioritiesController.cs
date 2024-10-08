﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unbugit.Data;
using Unbugit.Models;

namespace Unbugit.Controllers
{
    public class TicketPrioritiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketPrioritiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketPriorities
        public async Task<IActionResult> Index()
        {
            return View(await _context.TicketPriority.ToListAsync());
        }

        // GET: TicketPriorities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriority
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketPriority == null)
            {
                return NotFound();
            }

            return View(ticketPriority);
        }

        // GET: TicketPriorities/Create
        [Authorize(Roles="Admin, ProjectManager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TicketPriorities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> Create([Bind("Id,Name")] TicketPriority ticketPriority)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketPriority);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticketPriority);
        }

        // GET: TicketPriorities/Edit/5
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriority.FindAsync(id);
            if (ticketPriority == null)
            {
                return NotFound();
            }
            return View(ticketPriority);
        }

        // POST: TicketPriorities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TicketPriority ticketPriority)
        {
            if (id != ticketPriority.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketPriority);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketPriorityExists(ticketPriority.Id))
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
            return View(ticketPriority);
        }

        // GET: TicketPriorities/Delete/5
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriority
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketPriority == null)
            {
                return NotFound();
            }

            return View(ticketPriority);
        }

        // POST: TicketPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketPriority = await _context.TicketPriority.FindAsync(id);
            _context.TicketPriority.Remove(ticketPriority);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketPriorityExists(int id)
        {
            return _context.TicketPriority.Any(e => e.Id == id);
        }
    }
}
