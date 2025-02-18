using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Demo.PL.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}



		#region Register

		// /Account/Register
		public IActionResult Register()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser()
				{
					FirstName = model.FirstName,
					LastName = model.LastName,
					Email = model.Email,
					UserName = model.Email.Split('@')[0],
					IsAgree = model.IsAgree,
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
					return RedirectToAction(nameof(Login));
				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(model);
		}

		#endregion

		#region Login
		public IActionResult Login()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user is not null)
				{
					var flag = await _userManager.CheckPasswordAsync(user, model.Password);
					if (flag)
					{
						await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
						return RedirectToAction("Index", "Home");
					}
					ModelState.AddModelError(string.Empty, "Invalid Password");
				}
			}
			ModelState.AddModelError(string.Empty, "Email is not Existed");
			return View(model);

		}
		#endregion

		#region Sign Out


		public new async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}










		#endregion

		#region Forget Password

		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model) // Generate and send email with reset link
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user is not null)
				{
					var token = await _userManager.GeneratePasswordResetTokenAsync(user); // Token Valid for this user only once
					var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);
					//https://localhost:44392/Account/ResetPassword?email=0x0desha74@gmial.com
					var email = new Email()
					{
						Subject = "Reset Password",
						To = user.Email,
						Body = passwordResetLink
					};
					EmailSettings.SendEmail(email);
					return RedirectToAction("CheckYourInbox");
				}

				ModelState.AddModelError(string.Empty, "Email is not Existed");
			}
			return RedirectToAction(nameof(ForgetPassword));
		}

		public IActionResult CheckYourInbox()
		{
			return View();
		}
		#endregion

		#region Reset Password

		public IActionResult ResetPassword(string email, string token)
		{
		
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{ 

				string email = TempData["email"] as string;
				string token = TempData["token"] as string;

				var user = await _userManager.FindByEmailAsync(email);

				var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

				if (result.Succeeded)
					return RedirectToAction(nameof(Login));
				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}



		#endregion







	}












}
