using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PustokTask.Areas.Manage.Controllers
{
    [Area("Manage")]
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

        public async Task<IActionResult> CreateAdmin()
        {
            AppUser user = new AppUser
            {
                UserName = "Admin",
                Fulname = "Admin Admin",
                Email = "admin@gmail.com"

            };

            var result = await _userManager.CreateAsync(user , "_Admin123");
            return Json(result);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByEmailAsync(adminLoginVm.Username);

            if(user == null)
            {
                ModelState.AddModelError("", "invalid username or paswword");
                return View();
            }

            if (await _userManager.IsInRoleAsync(user, "Admin") ||! await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                ModelState.AddModelError("", "You are not allowed to Login here");
                return View();
            }
            var result = await _userManager.CheckPasswordAsync(user, adminLoginVm.Password);

            if (!result)
            {
                ModelState.AddModelError("", "invalid username or password");
                return View();
            }

            await _signInManager.SignInAsync(user, false);

            //var result = await _signInManager.CheckPasswordSignInAsync(user, adminLoginVm.Password, true);

            //if (result.Succeeded)
            //{
            //    ModelState.AddModelError("", "invalid username or password");
            //        return View();
            //}
            return RedirectToAction("index" , "Dashboard");

        }

        public async Task<IActionResult> LogOunt()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> CreateRole()
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Menber" });
            return Content("Role created");
        }
    }
}
