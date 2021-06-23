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
using System.Drawing;

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
            int companyId = currentUser.CompanyId;
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

        [HttpPost]
        public async Task<JsonResult> PieChartMethod()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            List<object> chartData = new();
            chartData.Add(new object[] { "ProjectName", "TicketCount" });

            foreach (Project prj in projects)
            {
                chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
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

        // ORIGINAL CHART/DONUT(PIE)
        [HttpPost]
        public async Task<JsonResult> DonutMethod()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<Project> projects = (await _projectService.GetAllProjectsByCompany(companyId)).OrderBy(p => p.Id).ToList();

            DonutViewModel chartData = new();
            chartData.labels = projects.Select(p => p.Name).ToArray();

            List<SubData> dsArray = new();
            List<int> tickets = new();
            List<string> colors = new();

            foreach (Project prj in projects)
            {
                tickets.Add(prj.Tickets.Count());

                // This code will randomly select a color for each element of the data 
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = tickets.ToArray(),
                backgroundColor = colors.ToArray()
            };
            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }
        // CHART/DONUT PRIORITY
        [HttpPost]
        public async Task<JsonResult> DonutMethodPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<Ticket> allTickets = new();
            List<Ticket> priority1 = (await _ticketService.GetAllTicketsByPriorityAsync((companyId), "Urgent")).OrderBy(p => p.Id).ToList();
            List<Ticket> priority2 = (await _ticketService.GetAllTicketsByPriorityAsync((companyId), "High")).OrderBy(p => p.Id).ToList();
            List<Ticket> priority3 = (await _ticketService.GetAllTicketsByPriorityAsync((companyId), "Medium")).OrderBy(p => p.Id).ToList();
            List<Ticket> priority4 = (await _ticketService.GetAllTicketsByPriorityAsync((companyId), "Low")).OrderBy(p => p.Id).ToList();
            foreach (Ticket item in priority1)
            {
                allTickets.Add(item);
            }
            foreach (Ticket item in priority1)
            {
                allTickets.Add(item);
            }
            foreach (Ticket item in priority1)
            {
                allTickets.Add(item);
            }
            foreach (Ticket item in priority1)
            {
                allTickets.Add(item);
            }

            DonutViewModel chartData = new();
            chartData.labels = allTickets.Select(p => p.Title).ToArray();

            List<SubData> dsArray = new();
            List<int> tickets = new();
            List<string> colors = new();

            foreach (Ticket ticket in allTickets)
            {
                tickets.Add(ticket.TicketPriority.Id);

                // This code will randomly select a color for each element of the data 
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = tickets.ToArray(),
                backgroundColor = colors.ToArray()
            };
            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }
        // CHART/DONUT STATUS
        //[HttpPost]
        //public async Task<JsonResult> DonutMethodStatus()
        //{
        //    int companyId = User.Identity.GetCompanyId().Value;
        //    Random rnd = new();

        //    List<Project> projects = (await _projectService.GetAllProjectsByCompany(companyId)).OrderBy(p => p.Id).ToList();

        //    DonutViewModel chartData = new();
        //    chartData.labels = projects.Select(p => p.Name).ToArray();

        //    List<SubData> dsArray = new();
        //    List<int> tickets = new();
        //    List<string> colors = new();

        //    foreach (Project prj in projects)
        //    {
        //        tickets.Add(prj.Tickets.Count());

        //        // This code will randomly select a color for each element of the data 
        //        Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        //        string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

        //        colors.Add(colorHex);
        //    }

        //    SubData temp = new()
        //    {
        //        data = tickets.ToArray(),
        //        backgroundColor = colors.ToArray()
        //    };
        //    dsArray.Add(temp);

        //    chartData.datasets = dsArray.ToArray();

        //    return Json(chartData);
        //}
        // CHART/DONUT TYPE
        //[HttpPost]
        //public async Task<JsonResult> DonutMethodType()
        //{
        //    int companyId = User.Identity.GetCompanyId().Value;
        //    Random rnd = new();

        //    List<Project> projects = (await _projectService.GetAllProjectsByCompany(companyId)).OrderBy(p => p.Id).ToList();

        //    DonutViewModel chartData = new();
        //    chartData.labels = projects.Select(p => p.Name).ToArray();

        //    List<SubData> dsArray = new();
        //    List<int> tickets = new();
        //    List<string> colors = new();

        //    foreach (Project prj in projects)
        //    {
        //        tickets.Add(prj.Tickets.Count());

        //        // This code will randomly select a color for each element of the data 
        //        Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        //        string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

        //        colors.Add(colorHex);
        //    }

        //    SubData temp = new()
        //    {
        //        data = tickets.ToArray(),
        //        backgroundColor = colors.ToArray()
        //    };
        //    dsArray.Add(temp);

        //    chartData.datasets = dsArray.ToArray();

        //    return Json(chartData);
        //}

    }
}
