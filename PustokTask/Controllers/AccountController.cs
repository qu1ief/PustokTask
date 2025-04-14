using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokTask.Models;
using PustokTask.ViewModels;

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

        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> LogOunt()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [Authorize(Roles = "Menber")]
    public async Task<IActionResult> Profile()
    {
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

    public async Task<IActionResult> Profile(UserUpdateProfile userUpdateProfileVM)
    {

        UserProfileVm userProfileVm = new UserProfileVm
        {

            UserUpdateProfilee = userUpdateProfileVM
        };
        if (!ModelState.IsValid)
        {
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();

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
        await _signInManager.SignInAsync(user,  true);
        return RedirectToAction("index", "Home");


    }

}
