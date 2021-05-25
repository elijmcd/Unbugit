﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unbugit.Data;
using Unbugit.Models;

namespace Unbugit.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketAttachmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TicketAttachment.Include(t => t.Ticket).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TicketAttachments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Create
        public IActionResult Create()
        {
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: TicketAttachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileName,FileData,FileContentType,Description,Created,TicketId,UserId")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Description", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment.FindAsync(id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Description", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,FileData,FileContentType,Description,Created,TicketId,UserId")] TicketAttachment ticketAttachment)
        {
            if (id != ticketAttachment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketAttachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketAttachmentExists(ticketAttachment.Id))
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
            ViewData["TicketId"] = new SelectList(_context.Ticket, "Id", "Description", ticketAttachment.TicketId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketAttachment = await _context.TicketAttachment
                .Include(t => t.Ticket)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketAttachment == null)
            {
                return NotFound();
            }

            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketAttachment = await _context.TicketAttachment.FindAsync(id);
            _context.TicketAttachment.Remove(ticketAttachment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketAttachmentExists(int id)
        {
            return _context.TicketAttachment.Any(e => e.Id == id);
        }
    }
}
