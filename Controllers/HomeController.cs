using Unbugit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Unbugit.Services.Interfaces;
using Unbugit.Data;
using Microsoft.AspNetCore.Authorization;
using Unbugit.Extensions;

namespace Unbugit.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRoleService _roleService;
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IBTProjectService projectService, IBTTicketService ticketService, IBTCompanyInfoService companyInfoService, IBTRoleService roleService, UserManager<BTUser> userManager)
        {
            _logger = logger;
            _projectService = projectService;
            _ticketService = ticketService;
            _companyInfoService = companyInfoService;
            _roleService = roleService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel dashboard = new();

            BTUser currentUser = await _userManager.GetUserAsync(User);
            int companyId = currentUser.CompanyId.Value;
            if (User.IsInRole("Admin"))
            {
                dashboard.Projects = await _projectService.GetAllProjectsByCompany(companyId);
            }
            else
            {
                dashboard.Projects = await _projectService.ListUserProjectsAsync(currentUser.Id);
            }

            dashboard.Tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);
            dashboard.Users = await _companyInfoService.GetAllMembersAsync(companyId);

            return View(dashboard);
        }

        public IActionResult Landing()
        {
            return View();
        }

        public async Task<JsonResult> PieChartMethod()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project project in projects)
            {
                chartData.Add(new object[] { project.Name, project.Tickets.Count() });
            }

            return Json(chartData);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
