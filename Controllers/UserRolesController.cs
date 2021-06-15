using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Data;
using Unbugit.Extensions;
using Unbugit.Models;
using Unbugit.Models.Enums;
using Unbugit.Models.ViewModels;
using Unbugit.Services.Interfaces;

namespace Unbugit.Controllers
{
    //[Authorize(Roles="Admin")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRoleService _roleService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTProjectService _projectService;

        public UserRolesController(ApplicationDbContext context,
                                   IBTRoleService roleService,
                                   IBTCompanyInfoService companyInfoService,
                                   IBTProjectService projectService) 
        {
            _context = context;
            _roleService = roleService;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            //TODO company users ... little more work to do
            List<BTUser> companyMembers = await _companyInfoService.GetAllMembersAsync(companyId);

            foreach (var user in companyMembers)
            {
                ManageUserRolesViewModel vm = new();
                vm.BTUser = user;
                var selected = await _roleService.ListUserRolesAsync(user);
                vm.Roles = new MultiSelectList(_context.Roles, "Name", "Name", selected);
                model.Add(vm);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            BTUser user = _context.Users.Find(member.BTUser.Id);

            IEnumerable<string> roles = await _roleService.ListUserRolesAsync(user);

            await _roleService.RemoveUserFromRolesAsync(user, roles);
            string userRole = member.SelectedRoles.FirstOrDefault();

            if(Enum.TryParse(userRole, out Roles roleValue))
            {
                await _roleService.AddUserToRoleAsync(user, userRole);
                return RedirectToAction("ManageUserRoles");
            }

            return RedirectToAction("ManageUserRoles");
        }
    }
}
