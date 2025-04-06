using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.Models;

namespace PustokTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize]


    public class SettingController(PustokDbContex pustokDbContex) : Controller
    {
        public IActionResult Index()
        {
            var settings = pustokDbContex.Settings.ToList();
            return View(settings);
            
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Setting setting)
        {
            if (!ModelState.IsValid) return View(setting);

            pustokDbContex.Settings.Add(setting);
            pustokDbContex.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
