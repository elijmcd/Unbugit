using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unbugit.Data;
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

        public UserRolesController(ApplicationDbContext context,
                                   IBTRoleService roleService) 
        {
            _context = context;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            //TODO company users ... little more work to do
            List<BTUser> users = _context.Users.ToList();

            foreach (var user in users)
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
