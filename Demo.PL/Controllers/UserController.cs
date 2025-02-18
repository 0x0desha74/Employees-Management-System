using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin,HR")]

    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }



        public async Task<IActionResult> Index(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                var users = await _userManager.Users.Select(U => new UserViewModel
                {
                    Id = U.Id,
                    FirstName = U.FirstName,
                    LastName = U.LastName,
                    Email = U.Email,
                    UserName = U.UserName,
                    PhoneNumber = U.PhoneNumber,
                    Roles = _userManager.GetRolesAsync(U).Result
                }).ToListAsync();

                return View(users);
            }
            else
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var mappedUser = new UserViewModel()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = _userManager.GetRolesAsync(user).Result
                    };

                    return View(new List<UserViewModel>() { mappedUser });
                }
                return View(Enumerable.Empty<UserViewModel>());

            }
        }

        // /User/Details/Guid
        //[HttpGet]

        public async Task<IActionResult> Details(string Id, string ViewName = "Details")
        {
            if (Id is null)
                return BadRequest();
            else
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user is null)
                    return NotFound();
                else
                {
                    var mappedUser = _mapper.Map<ApplicationUser, UserViewModel>(user);
                    return View(ViewName, mappedUser);
                }
            }
        }

        // /User/Edit/Guid
        //[HttpGet]

        public async Task<IActionResult> Edit(string Id)
        {
            return await Details(Id, "Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string Id, UserViewModel updatedUser)
        {
            if (Id != updatedUser.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(Id);
                    user.FirstName = updatedUser.FirstName;
                    user.LastName = updatedUser.LastName;
                    user.PhoneNumber = updatedUser.PhoneNumber;
                    user.Email = updatedUser.Email;
                    user.SecurityStamp = Guid.NewGuid().ToString();

                    await _userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(updatedUser);
        }


        // /User/Delete/Guid
        //[HttpGet]
        public async Task<IActionResult> Delete(string Id)
        {
            return await Details(Id, "Delete");
        }


        // /User/Delete/Guid
        //[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string Id)
        {

            try
            {
                var user = await _userManager.FindByIdAsync(Id);

                await _userManager.DeleteAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

    }



}
