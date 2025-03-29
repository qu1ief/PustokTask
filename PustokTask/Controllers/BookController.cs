using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    public class BookController(PustokDbContex pustokDbContex) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int? id)
        {
            if (id == null)
                return NotFound();
            var existBooks = pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b =>b.BookTags)
                .ThenInclude(b=>b.Tag)
                .FirstOrDefault(b => b.Id == id);
            if (existBooks == null)
                return NotFound();

            BookDetailVm vm = new()
            {
                Book = existBooks,
                RelatedBooks = pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Where(b => b.GenreId == id)
                .ToList()
            };
            return View(vm);
        }
        public IActionResult BookModal(int? id)
        {
            if (id == null)
                return NotFound();
            var existBooks = pustokDbContex.Books
              
                .FirstOrDefault(b => b.Id == id);
            if (existBooks == null)
                return NotFound();


            return PartialView("_ModalBookPartial", existBooks);
        }
    }
}
