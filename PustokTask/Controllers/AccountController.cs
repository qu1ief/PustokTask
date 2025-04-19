using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.Services;
using PustokTask.ViewModels;

namespace PustokTask.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<AppUser> _userManager;
	private readonly SignInManager<AppUser> _signInManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly EmailService emailService;
    private readonly IConfiguration _configuration;		
	private readonly PustokDbContex pustokDbContex;


    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, EmailService emailService, IConfiguration configuration, PustokDbContex pustokDbContex)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        this.emailService = emailService;
        _configuration = configuration;
        this.pustokDbContex = pustokDbContex;
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

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		var url = Url.Action("VerifyEmail", "Account", new { email = user.Email, token = token }, Request.Scheme);

		

		using StreamReader streamReader = new StreamReader("wwwroot/templates/verify.html");
		string body = await streamReader.ReadToEndAsync();
		body = body.Replace("{{url}}", url);
		body = body.Replace("{{usarname}}", user.Fulname);
		 emailService.SendEmail(user.Email, "Verify Email", body);


		return RedirectToAction("login");
	}
	public IActionResult VerifyEmail(string email , string token)
	{
		if(email == null || token == null)
		{
			return NotFound();
		}
		var user =  _userManager.FindByEmailAsync(email).Result;
		if (user == null)
		{
			return NotFound();
		}

		var result = _userManager.ConfirmEmailAsync(user ,token).Result;
		if (!result.Succeeded)
		{
			return NotFound();
		}
		return RedirectToAction("Login");
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

		if (!user.EmailConfirmed)
		{
			ModelState.AddModelError("" , "verify email");
		}
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

		Response.Cookies.Delete("basket");

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
			UserUpdateProfilee = userUpdateProfile,
			Orders = pustokDbContex.Orders.Where(o => o.AppUserId == user.Id).Include(o => o.Items).ThenInclude(i => i.Book).ToList(),

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

        using StreamReader streamReader = new StreamReader("wwwroot/templates/forgotpassword.html");
        string body = await streamReader.ReadToEndAsync();

        body = body.Replace("{{url}}", url);
        body = body.Replace("{{username}}", user.Fulname); 
        var emailSettings = _configuration.GetSection("Email").Get<EmailSettings>();

        emailService.SendEmail(user.Email, "reset password", body);

        return RedirectToAction("login");
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
