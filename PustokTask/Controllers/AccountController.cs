using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using PustokTask.Models;
using PustokTask.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using NuGet.Protocol;

namespace PustokTask.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_roleManager = roleManager;
	}
	[HttpGet]
	public async Task<IActionResult> Register()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Register(UserRegisterVm userRegister)
	{
		if (!ModelState.IsValid)
		{
			return View(userRegister);
		}

		AppUser user = await _userManager.FindByNameAsync(userRegister.UserName);

		if (user != null)
		{
			ModelState.AddModelError("UserName", "user already exsist ...");
		}

		user = new AppUser
		{
			Fulname = userRegister.FullName,
			Email = userRegister.Email,
			UserName = userRegister.UserName

		};

		var result = await _userManager.CreateAsync(user, userRegister.Password);
		await _userManager.AddToRoleAsync(user, "Menber");


		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View();
		}


		return RedirectToAction("login");
	}


	[HttpGet]
	public async Task<IActionResult> Login()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Login(UserLoginVm vm, string returnUrl)
	{
		if (!ModelState.IsValid)
		{
			return View(vm);
		}

		var user = await _userManager.FindByNameAsync(vm.UserNameOrEmail);
		if (user == null)
		{
			user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail);
			if (user == null)
			{
				ModelState.AddModelError("", "Invalid username or email");
				return View(vm);
			}
		}

		if (await _userManager.IsInRoleAsync(user, "Admin"))
		{
			ModelState.AddModelError("", "You are not allowed to Login here");
			return View();
		}

		var r = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RebemberMe, true);

		if (r.IsLockedOut)
		{
			ModelState.AddModelError("", "Account is locked out");
			return View(vm);
		}

		if (!r.Succeeded)
		{
			ModelState.AddModelError("", "Invalid password or username");
			return View(vm);
		}

		return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
	}


	public async Task<IActionResult> LogOunt()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction("Login");
	}

	[Authorize(Roles = "Menber")]
	public async Task<IActionResult> Profile(string tab = "Dashboard")
	{
		ViewBag.Tab = tab;

		var user = await _userManager.GetUserAsync(User);
		UserUpdateProfile userUpdateProfile = new UserUpdateProfile
		{
			FullName = user.Fulname,
			UserName = user.UserName,
			Email = user.Email,
		};

		UserProfileVm vm = new UserProfileVm
		{
			UserUpdateProfilee = userUpdateProfile
		};

		return View(vm);
	}
	[Authorize(Roles = "Menber")]
	[HttpPost]

	public async Task<IActionResult> Profile(UserUpdateProfile userUpdateProfileVM, string tab = "Profile")
	{
		ViewBag.Tab = tab;
		UserProfileVm userProfileVm = new UserProfileVm
		{

			UserUpdateProfilee = userUpdateProfileVM
		};
		if (!ModelState.IsValid)
		{
			return View(userProfileVm);
		}

		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound();

		}
		if (userUpdateProfileVM.NewPassword != null)
		{
			if (userUpdateProfileVM.CurentPassword == null)
			{
				ModelState.AddModelError("CuurentPassword", "Current password is requered");
				return View(userUpdateProfileVM);
			}
			else
			{
				var passwordUpdateResult = await _userManager.ChangePasswordAsync(user, userUpdateProfileVM.CurentPassword, userUpdateProfileVM.NewPassword);

				if (passwordUpdateResult.Succeeded)
				{
					foreach (var errors in passwordUpdateResult.Errors)
					{
						ModelState.AddModelError("", errors.Description);
					}
					return View(userUpdateProfileVM);
				}
			}
		}
		user.Fulname = userUpdateProfileVM.FullName;
		user.UserName = userUpdateProfileVM.UserName;
		user.Email = userUpdateProfileVM.Email;
		var result = await _userManager.UpdateAsync(user);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View();
		}
		await _signInManager.SignInAsync(user, true);
		return RedirectToAction("index", "Home");


	}


	public async Task<IActionResult> ForgotPassword()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordVm vm)
	{
		if (!ModelState.IsValid)
		{
			return View();
		}

		var user = await _userManager.FindByEmailAsync(vm.Email);

		if (user == null)
		{
			ModelState.AddModelError("", "Email not found");
			return View();
		}
		var token = await _userManager.GeneratePasswordResetTokenAsync(user);
		var url = Url.Action("ResertPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);

		var email = new MimeMessage();
		email.From.Add(MailboxAddress.Parse("allupproje@gmail.com"));
		email.To.Add(MailboxAddress.Parse("myrtle.waters@ethereal.email"));
		email.Subject = "Resert Password";
		email.Body = new TextPart(TextFormat.Html) { Text = $"< a href = '{url}' >click</a>" };

		using var smtp = new SmtpClient();
		smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
		smtp.Authenticate("allupproje@gmail.com", "srge clqo huup dipv");
		smtp.Send(email);
		smtp.Disconnect(true);
		return Json(url);
		return RedirectToAction("index");


	}
	public async Task<IActionResult> ResertPassword()
	{
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> ResertPassword(ResertPasswordVm vm)
	{
		if (ModelState.IsValid)
		{
			return View(vm);

		}
		var user = await _userManager.FindByEmailAsync(vm.Email);
		if (user == null)
		{
			ModelState.AddModelError("", "Email not found");
		}

		var result = await _userManager.ResetPasswordAsync(user, vm.Token, vm.NewPassword);
		if (result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);

			}
			return View(vm);
		}
		return RedirectToAction("Login");
	}

}
