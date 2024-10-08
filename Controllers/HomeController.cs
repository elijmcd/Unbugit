﻿using Unbugit.Models;
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
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
            IBTProjectService projectService,
            IBTTicketService ticketService,
            IBTCompanyInfoService companyInfoService,
            IBTRoleService roleService,
            UserManager<BTUser> userManager,
            ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _projectService = projectService;
            _ticketService = ticketService;
            _companyInfoService = companyInfoService;
            _roleService = roleService;
            _userManager = userManager;
            _context = applicationDbContext;
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
            int companyId = (int)currentUser.CompanyId;

            dashboard.Projects = await _projectService.GetAllProjectsByCompany(companyId);
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
        // CHART/BAR PRIORITY
        [HttpPost]
        public async Task<JsonResult> DonutMethodPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<TicketPriority> priorities = _context.TicketPriority.ToList();
            List<Ticket> allTickets = (await _companyInfoService.GetAllTicketsAsync(companyId)).OrderBy(t => t.TicketPriorityId).ToList();

            DonutViewModel chartData = new();
            chartData.labels = priorities.Select(p => p.Name).ToArray();


            List<SubData> dsArray = new();
            List<int> pTickets = new();
            //List<string> colors = new();
            List<string> bgcolors = new();

            bgcolors.Add("rgba(255, 62, 108, 0.8)");
            bgcolors.Add("rgba(255, 204, 62, 0.8)");
            bgcolors.Add("rgba(48, 190, 251, 0.8)");
            bgcolors.Add("rgba(80, 102, 225, 0.8)");

            foreach (TicketPriority priority in priorities)
            {
                pTickets.Add(allTickets.Where(t => t.TicketPriorityId == priority.Id).Count());

                // This code will randomly select a color for each element of the data 
                //Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                //string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

                //colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = pTickets.ToArray(),
                backgroundColor = bgcolors.ToArray()
            };
            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }
        //CHART/DONUT TYPE
        [HttpPost]
        public async Task<JsonResult> DonutMethodType()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<TicketType> types = _context.TicketType.ToList();
            List<Ticket> allTickets = (await _companyInfoService.GetAllTicketsAsync(companyId)).OrderBy(t => t.TicketTypeId).ToList();

            DonutViewModel chartData = new();
            chartData.labels = types.Select(p => p.Name).ToArray();


            List<SubData> dsArray = new();
            List<int> tTickets = new();
            List<string> colors = new();

            foreach (TicketType type in types)
            {
                tTickets.Add(allTickets.Where(t => t.TicketTypeId == type.Id).Count());

                // This code will randomly select a color for each element of the data 
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = tTickets.ToArray(),
                backgroundColor = colors.ToArray()
            };
            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }
        //CHART/DONUT STATUS
        [HttpPost]
        public async Task<JsonResult> DonutMethodStatus()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            Random rnd = new();

            List<TicketStatus> statuses = _context.TicketStatus.ToList();
            List<Ticket> allTickets = (await _companyInfoService.GetAllTicketsAsync(companyId)).OrderBy(t => t.TicketStatusId).ToList();

            DonutViewModel chartData = new();
            chartData.labels = statuses.Select(p => p.Name).ToArray();


            List<SubData> dsArray = new();
            List<int> sTickets = new();
            List<string> colors = new();

            foreach (TicketStatus status in statuses)
            {
                sTickets.Add(allTickets.Where(t => t.TicketStatusId == status.Id).Count());

                // This code will randomly select a color for each element of the data 
                Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                string colorHex = string.Format("#{0:X6}", randomColor.ToArgb() & 0X00FFFFFF);

                colors.Add(colorHex);
            }

            SubData temp = new()
            {
                data = sTickets.ToArray(),
                backgroundColor = colors.ToArray()
            };
            dsArray.Add(temp);

            chartData.datasets = dsArray.ToArray();

            return Json(chartData);
        }

    }
}
