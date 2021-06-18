using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Unbugit.Data;
using Unbugit.Extensions;
using Unbugit.Models;
using Unbugit.Models.Enums;
using Unbugit.Models.ViewModels;
using Unbugit.Services.Interfaces;

namespace Unbugit.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRoleService _roleService;

        public ProjectsController(ApplicationDbContext context,
            IBTProjectService projectService,
            IBTCompanyInfoService companyInfoService,
            UserManager<BTUser> userManager,
            IBTRoleService roleService
            )
        {
            _context = context;
            _projectService = projectService;
            _companyInfoService = companyInfoService;
            _userManager = userManager;
            _roleService = roleService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Project.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Members)
                .Include(p => p.Company)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketPriority)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                    .ThenInclude(t => t.TicketType)
                .Include(p=>p.Tickets)
                    .ThenInclude(t => t.OwnerUser)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public async Task<IActionResult> MyProjects()
        {
            string userId = (await _userManager.GetUserAsync(User)).Id;

            List<Project> myProjects = await _projectService.ListUserProjectsAsync(userId);

            return View(myProjects);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> CreateAsync()
        {
            //get current user
            BTUser btUser = await _userManager.GetUserAsync(User);

            // get current user's company Id
            int companyId = User.Identity.GetCompanyId().Value;

            if ((User.IsInRole("Admin")) || (User.IsInRole("ProjectManager")))
            {
                ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Name");
            }
            else
            {
                //ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name");
                return RedirectToAction("Index");
            }

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CompanyId,Description,StartDate,EndDate,ProjectPriority,ProjectPriorityId")] Project project) //CompanyId,ImageFileName,ImageFileData,ImageContentType,Archived
        {
            BTUser btUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                project.StartDate = DateTimeOffset.Now;
                project.Company = btUser.Company;
                project.CompanyId = (btUser.CompanyId).Value;

                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Projects", new { id = project.Id });
            }
            //ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Description,StartDate,EndDate,ProjectPriority,ProjectPriorityId,ImageFileName,ImageFileData,ImageContentType,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("MyProjects");
            }
            ViewData["CompanyId"] = new SelectList(_context.Company, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.Set<ProjectPriority>(), "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        //GET Users/Assign
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignUsers(int id)
        {
            ProjectMembersViewModel model = new();

            //get companyId
            int companyId = User.Identity.GetCompanyId().Value;

            Project project = (await _projectService.GetAllProjectsByCompany(companyId))
                                                    .FirstOrDefault(p => p.Id == id);

            model.Project = project;
            List<BTUser> developers = await _companyInfoService.GetMembersInRoleAsync(Roles.Developer.ToString(), companyId);
            List<BTUser> submitters = await _companyInfoService.GetMembersInRoleAsync(Roles.Submitter.ToString(), companyId);

            List<BTUser> users = developers.Concat(submitters).ToList();
            List<string> members = project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);
        }

        //POST Users/Assign
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUsers(ProjectMembersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    List<string> memberIds = (await _projectService.GetMembersWithoutPMAsync(model.Project.Id))
                                                                    .Select(m => m.Id).ToList();

                    foreach (string id in memberIds)
                    {
                        await _projectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                    }

                    foreach (string id in model.SelectedUsers)
                    {
                        await _projectService.AddUserToProjectAsync(id, model.Project.Id);
                    }
                    //go to Project 'Details' instead
                    return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
                }
                else
                {
                    //send an error message
                }
            }
            return View(model);
        }

        //GET Users/AssignPM
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AssignPM(int id)
        {
            AssignPMViewModel model = new();

            //get companyId
            int companyId = User.Identity.GetCompanyId().Value;

            Project project = (await _projectService.GetAllProjectsByCompany(companyId))
                                                    .FirstOrDefault(p => p.Id == id);

            model.Project = project;
            List<BTUser> projectManagers = await _companyInfoService.GetMembersInRoleAsync(Roles.ProjectManager.ToString(), companyId);

            model.Users = new SelectList(projectManagers, "Id", "FullName");

            return View(model);
        }

        //POST Users/AssignPM
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (ModelState.IsValid)
            {

                await _projectService.AddProjectManagerAsync(model.PMId, model.Project.Id);
                
                //go to Project 'Details' instead
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }
            else
            {
                //send an error message
            }
            return View();
        }

        //GET Users/RemoveUsers
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> RemoveUsers(int id)
        {
            ProjectMembersViewModel model = new();

            //get companyId
            int companyId = User.Identity.GetCompanyId().Value;

            Project project = (await _projectService.GetAllProjectsByCompany(companyId))
                                                    .FirstOrDefault(p => p.Id == id);

            model.Project = project;
            List<BTUser> developers = await _companyInfoService.GetMembersInRoleAsync(Roles.Developer.ToString(), companyId);
            List<BTUser> submitters = await _companyInfoService.GetMembersInRoleAsync(Roles.Submitter.ToString(), companyId);

            List<BTUser> users = developers.Concat(submitters).ToList();
            List<string> members = project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);
        }

        //POST Users/RemoveUsers
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUsers(ProjectMembersViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.SelectedUsers != null)
                {
                    List<string> memberIds = (await _projectService.GetMembersWithoutPMAsync(model.Project.Id))
                                                                    .Select(m => m.Id).ToList();

                    //foreach (string id in memberIds)
                    //{
                    //    await _projectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                    //}

                    foreach (string id in model.SelectedUsers)
                    {
                        await _projectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                    }

                    //go to Project 'Details' instead
                    return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
                }
                else
                {
                    //send an error message
                }
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Project.FindAsync(id);
            _context.Project.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }
    }
}
