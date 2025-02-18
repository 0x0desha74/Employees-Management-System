using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
        }

        // /Role/Index/Guid

        public async Task<IActionResult> Index(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                var Roles = await _roleManager.Roles.Select(R => new RoleViewModel
                {
                    Id = R.Id,
                    RoleName = R.Name,
                }).ToListAsync();
                return View(Roles);
            }
            else
            {
                var Role = await _roleManager.FindByNameAsync(name);
                if (Role is not null)
                {
                    var mappedRole = new RoleViewModel()
                    {
                        Id = Role?.Id,
                        RoleName = Role?.Name,
                    };
                    return View(new List<RoleViewModel> { mappedRole });
                }
                return View(Enumerable.Empty<RoleViewModel>());
            }
        }

        // /Role/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                var mappedRole = _mapper.Map<RoleViewModel, IdentityRole>(roleVM);
                await _roleManager.CreateAsync(mappedRole);
                return RedirectToAction(nameof(Index));
            }
            return View(roleVM);
        }


        // /Role/Details/Guid
        public async Task<IActionResult> Details(string Id, string ViewName = "Details")
        {
            if (Id is null)
                return BadRequest();
            var Role = await _roleManager.FindByIdAsync(Id);
            if (Role is null)
                return NotFound();
            var mappedRole = _mapper.Map<IdentityRole, RoleViewModel>(Role);
            return View(ViewName, mappedRole);
        }

        // /Role/Edit.Guid
        public async Task<IActionResult> Edit(string Id)
        {
            return await Details(Id, "Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string Id, RoleViewModel updatedRole)
        {
            if (Id != updatedRole.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(Id);
                    role.Id = updatedRole.Id;
                    role.Name = updatedRole.RoleName;
                    await _roleManager.UpdateAsync(role);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(updatedRole);

        }


        // /Role/Delete/Guid

        public async Task<IActionResult> Delete(string Id)
        {
            return await Details(Id, "Delete");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string Id)
        {
            if (Id is null)
                return BadRequest();

            try
            {
                var role = await _roleManager.FindByIdAsync(Id);
                await _roleManager.DeleteAsync(role);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Error", "Home");

            }
        }


        public async Task<IActionResult> AddOrRemoveUsers(string RoleId)
        {
            if (string.IsNullOrEmpty(RoleId))
                return BadRequest();
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is null)
                return NotFound();
            ViewBag.RoleId = RoleId;
            var users = new List<UserInRoleViewModel>();
            foreach (var user in _userManager.Users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userInRole.IsSelected = true;
                }
                else
                {
                    userInRole.IsSelected = false;
                }
                users.Add(userInRole);
            }


            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrRemoveUsers(List<UserInRoleViewModel> models, string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
            if (role is null)
                return NotFound();
            if (ModelState.IsValid)
            {
                foreach (var item in models)
                {
                    var user = await _userManager.FindByIdAsync(item.UserId);
                    if (user is not null)
                    {
                        if (item.IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                            await _userManager.AddToRoleAsync(user, role.Name);
                        else if (!item.IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                            await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }

                }
            }
            return RedirectToAction("Edit", new { Id = RoleId });
        }



    }
}
