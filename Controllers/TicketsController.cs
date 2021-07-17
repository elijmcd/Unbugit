using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unbugit.Data;
using Unbugit.Models;
using Unbugit.Extensions;
using Unbugit.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Unbugit.Models.Enums;
using Unbugit.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Unbugit.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTTicketService _ticketService;
        private readonly IBTCompanyInfoService _companyService;
        private readonly IBTRoleService _roleService;
        private readonly IBTProjectService _projectService;
        private readonly IBTHistoryService _historyService;
        private readonly IBTNotificationService _notificationService;


        public TicketsController(ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<BTUser> userManager,
            IBTTicketService ticketService,
            IBTCompanyInfoService companyService,
            IBTRoleService roleService,
            IBTProjectService projectService,
            IBTHistoryService historyService,
            IBTNotificationService notificationService)
        {
            _context = context;
            _ticketService = ticketService;
            _companyService = companyService;
            _roleService = roleService;
            _roleManager = roleManager;
            _userManager = userManager;
            _projectService = projectService;
            _historyService = historyService;
            _notificationService = notificationService;
        }

        //GET: Tickets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ticket
                                        .Include(t => t.DeveloperUser)
                                        .Include(t => t.OwnerUser)
                                        .Include(t => t.Project)
                                        .Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.Attachments)
                                        .Include(t => t.TicketType).ToListAsync();

            return View(await applicationDbContext);
        }

        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> tickets = new();

                 tickets = await _companyService.GetAllTicketsAsync(companyId);

            return View(tickets);
        }

        public async Task<IActionResult> Unassigned()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> tickets = new();

                tickets = await _companyService.GetUnassignedTicketsAsync(companyId);

            return View(tickets);
        }

        public async Task<IActionResult> MyTickets()
        {
            string userId = (await _userManager.GetUserAsync(User)).Id;
            int companyId = User.Identity.GetCompanyId().Value;
            List<Ticket> companyTickets = await _companyService.GetAllTicketsAsync(companyId);
            List<Ticket> submitterTickets = companyTickets.Where(t => t.OwnerUserId == userId).ToList();
            List<Ticket> developerTickets = companyTickets.Where(t => t.DeveloperUserId == userId).ToList();

            List<Ticket> myTickets = developerTickets.Concat(submitterTickets).ToList();

            return View(myTickets);
        }

        //GET: Tickets/Assign
        [HttpGet]
        [Authorize(Roles="Admin,ProjectManager")]
        public async Task<IActionResult> AssignTicket(int? ticketId)
        {
            if (!ticketId.HasValue)
            {
                return NotFound();
            }
            AssignDeveloperViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Ticket = (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);
            //await _context.Ticket.FirstOrDefaultAsync(t => t.Id == ticketId);
            //
            model.Developers = new SelectList(await _projectService.DevelopersOnProjectAsync(model.Ticket.ProjectId), "Id", "FullName");
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            return View(model);
        }
        //POST: Tickets/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin,ProjectManager")]
        public async Task<IActionResult> AssignTicket(AssignDeveloperViewModel viewModel, string returnUrl)
        {
            if (!string.IsNullOrEmpty(viewModel.DeveloperId))
            {
                int companyId = User.Identity.GetCompanyId().Value;
                Notification notification = new();

                BTUser currentUser = await _userManager.GetUserAsync(User);
                BTUser developer = (await _companyService.GetAllMembersAsync(companyId)).FirstOrDefault(m => m.Id == viewModel.DeveloperId);
                BTUser projectManager = await _projectService.GetProjectManagerAsync(viewModel.Ticket.ProjectId);

                Ticket oldTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                await _ticketService.AssignTicketAsync(viewModel.Ticket.Id, viewModel.DeveloperId);

                Ticket newTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == viewModel.Ticket.Id);

                notification = new()
                {
                    TicketId = newTicket.Id,
                    Title = "New Developer Ticket",
                    Message = $"You have a new ticket: {newTicket?.Title}, was created by {currentUser?.FullName}",
                    Created = DateTimeOffset.Now,
                    SenderId = currentUser.Id,
                    RecipientId = newTicket.DeveloperUserId
                };

                await _notificationService.SaveNotificationAsync(notification);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, currentUser.Id);
            }
            return Redirect(returnUrl);

            //return RedirectToAction("Details", new { id = viewModel.Ticket.Id });
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.Comments)
                    .ThenInclude(t => t.User)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Attachments)
                .Include(t => t.Notifications)
                .Include(t => t.TicketHistory)
                    .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            //get current user
            BTUser btUser = await _userManager.GetUserAsync(User);

            // get current user's company Id
            int companyId = User.Identity.GetCompanyId().Value;

            //TODO Filter List
            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompany(companyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] = new SelectList(await _projectService.ListUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name");
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();

            return PartialView();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string returnUrl, [Bind("Id,Title,OwnerUser,OwnerUserId,Description,ProjectId,TicketTypeId,TicketType,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                ticket.Created = DateTimeOffset.Now;

                ticket.OwnerUser = btUser;
                ticket.OwnerUserId = btUser.Id;

                ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync("New")).Value;

                await _context.AddAsync(ticket);
                await _context.SaveChangesAsync();

                #region Add History
                //Add history
                Ticket newTicket = await _context.Ticket
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                await _historyService.AddHistoryAsync(null, newTicket, btUser.Id);
                #endregion

                #region Notification
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);
                int companyId = User.Identity.GetCompanyId().Value;

                Notification notification = new()
                {
                    TicketId = ticket.Id,
                    Title = "New Ticket",
                    Message = $"New Ticket: {ticket?.Title}, created by {btUser?.FullName}",
                    Created = DateTimeOffset.Now,
                    SenderId = btUser?.Id,
                    RecipientId = projectManager?.Id
                };

                if (projectManager != null)
                {

                    await _notificationService.SaveNotificationAsync(notification);
                    await _notificationService.EmailNotificationAsync(notification, "New Ticket Added");

                }
                //else
                //{
                //    //notify Admin
                //    await _notificationService.AdminsNotificationAsync(notification, companyId);
                //}

                #endregion

                return Redirect(returnUrl);
            }
            //ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            //ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", ticket.ProjectId);
            //ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Id", ticket.TicketPriorityId);
            //ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Id", ticket.TicketStatusId);
            //ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Id", ticket.TicketTypeId);
            
            return Redirect(returnUrl);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(await _projectService.DevelopersOnProjectAsync(ticket.ProjectId), "Id", "FullName", ticket.DeveloperUserId);
            //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);

            ViewBag.returnUrl = Request.Headers["Referer"].ToString();
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string returnUrl, [Bind("Id,Title,Description,Created,Updated,ArchivedDate,Archived,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            Notification notification;

            if (ModelState.IsValid)
            {
                //companyId
                int companyId = User.Identity.GetCompanyId().Value;
                //currentUser
                BTUser currentUser = await _userManager.GetUserAsync(User);
                //projectManager
                BTUser projectManager = await _projectService.GetProjectManagerAsync(ticket.ProjectId);

                Ticket oldTicket = await _context.Ticket.Include(t => t.TicketPriority)
                                                        .Include(t => t.TicketStatus)
                                                        .Include(t => t.TicketType)
                                                        .Include(t => t.Comments)
                                                        .Include(t => t.Attachments)
                                                        .Include(t => t.Project)
                                                        .Include(t => t.DeveloperUser)
                                                        .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                try
                {
                    if (ticket.Archived == true)
                    {
                        ticket.ArchivedDate = DateTimeOffset.Now;
                        ticket.TicketStatusId = 1;
                    }

                    ticket.Updated = DateTimeOffset.Now;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();

                    //create and save a notification
                    notification = new()
                    {
                        TicketId = ticket.Id,
                        Title = $"Ticket modified in {oldTicket.Project.Name}",
                        Message = $"Ticket [{ticket.Id}]:{ticket.Title} updated by {currentUser?.FullName}",
                        Created = DateTimeOffset.Now,
                        SenderId = currentUser?.Id,
                        RecipientId = projectManager?.Id
                    };

                    if (projectManager != null)
                    {
                        //PM notify
                        await _notificationService.SaveNotificationAsync(notification);
                    }
                    else
                    {
                        //admin notify
                        await _notificationService.AdminsNotificationAsync(notification, companyId);
                    }

                    //alert dev if ticket is assigned
                    if (ticket.DeveloperUserId != null)
                    {
                        notification = new()
                        {
                            TicketId = ticket.Id,
                            Title = "A ticket assigned to you has been modified",
                            Message = $"Ticket [{ticket.Id}]:{ticket.Title} updated by {currentUser?.FullName}",
                            Created = DateTimeOffset.Now,
                            SenderId = currentUser.Id,
                            RecipientId = ticket.DeveloperUserId
                        };
                        await _notificationService.SaveNotificationAsync(notification);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //add history
                Ticket newTicket = await _context.Ticket.Include(t => t.TicketPriority)
                                        .Include(t => t.TicketStatus)
                                        .Include(t => t.TicketType)
                                        .Include(t => t.Project)
                                        .Include(t => t.DeveloperUser)
                                        .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                await _historyService.AddHistoryAsync(oldTicket, newTicket, currentUser.Id);

                return Redirect(returnUrl);
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.Set<TicketPriority>(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.Set<TicketStatus>(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.Set<TicketType>(), "Id", "Name", ticket.TicketTypeId);
            return View("/Tickets/Details", ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            _context.Ticket.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }

        public IActionResult ShowFile(int id)
        {
            TicketAttachment ticketAttachment = _context.TicketAttachment.Find(id);
            string fileName = ticketAttachment.FileName;
            byte[] fileData = ticketAttachment.FileData;
            string ext = Path.GetExtension(fileName).Replace(".", "");

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileName}");
            return File(fileData, $"application/{ext}");
        }
    }
}
