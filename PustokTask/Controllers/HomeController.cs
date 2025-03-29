using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    public class HomeController(PustokDbContex pustokDbContex) : Controller
    {

        public IActionResult Index()
        {
            HomeVm vm = new HomeVm
            {
                Sliders = pustokDbContex.Sliders.ToList(),
                NewBooks = pustokDbContex.Books
                 .Where(a => a.IsNew)
                 .Include(b => b.Author)
                 .Include(b => b.BookImages.Where(ab => ab.Status != null))
                 .ToList(),

                DiscountBooks = pustokDbContex.Books
                  .Where(a => a.DiscountPercentage > 0)
                  .Include(b => b.Author)
                  .Include(b => b.BookImages.Where(ab => ab.Status != null))
                  .ToList(),
                FeaturedBooks = pustokDbContex.Books
                .Where(a => a.IsFeatured)
                 .Include(b => b.Author)
                 .Include(b => b.BookImages.Where(ab => ab.Status != null))
                 .ToList(),



            };
            return View(vm);
        }

        


    }
}
