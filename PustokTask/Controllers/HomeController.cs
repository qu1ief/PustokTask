using Microsoft.AspNetCore.Mvc;
using PustokTask.Models;
using System.Diagnostics;

namespace PustokTask.Controllers
{
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
