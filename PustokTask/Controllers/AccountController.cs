using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PustokTask.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;

	public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
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
	public async Task<IActionResult> Login(UserLoginVm vm)
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

		return RedirectToAction("Index", "Home");
	}


	public async Task<IActionResult> LogOunt()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction("Login");
	}

}
