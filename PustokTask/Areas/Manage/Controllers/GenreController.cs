using Microsoft.AspNetCore.Mvc;
using PustokTask.Data;

namespace PustokTask.Areas.Manage.Controllers
{
    [Area("Manage")]

    public class GenreController(PustokDbContex pustokDbContex) : Controller
    {
        public IActionResult Index()
        {
            var genres  = pustokDbContex.Genres.ToList();
            return View(genres);
        }
    }
}
