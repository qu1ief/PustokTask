using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    public class BookController(PustokDbContex pustokDbContex,
                                   UserManager<AppUser> userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return NotFound();

            var user = userManager.GetUserAsync(User).Result;

            if (user != null)
            {
                var vm = await GetBookDetailVm((int)id, user.Id);
                if (vm.Book == null)
                    return NotFound();
                return View(vm);
            }
            else
            {
                var vm = await GetBookDetailVm((int)id);
                if (vm.Book == null)
                    return NotFound();
                return View(vm);
            }
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
        [HttpPost]
        //[Authorize(Roles ="Menber")]
        public async Task<IActionResult> AddComment(BookComment bookComment)
        {
            if (!pustokDbContex.Books.Any(b => b.Id == bookComment.BookId))
            {
                return NotFound();
            }

            var user = userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return RedirectToAction("login", "account", new { returnurl = Url.Action("detail", "book", new { id = bookComment.BookId }) });
            }


            if (!ModelState.IsValid)
            {
                var vm = await GetBookDetailVm(bookComment.BookId, user.Id);
                vm.BookComment = bookComment;
                return View("Detail", vm);
            }

            bookComment.AppUserId = user.Id;
            pustokDbContex.BookComments.Add(bookComment);
            await pustokDbContex.SaveChangesAsync();
            return RedirectToAction(Url.Action("detail", "book", new { id = bookComment.BookId }));
        }

        private async Task<BookDetailVm> GetBookDetailVm(int bookId, string userId = null)
        {

            var existBooks = await pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.BookComments.Where(bc => bc.Status == CommentStatus.Approved))
                .Include(b => b.BookTags)
                .ThenInclude(b => b.Tag)
                .FirstOrDefaultAsync(b => b.Id == bookId);


            BookDetailVm bookDetailVm = new()
            {
                Book = existBooks,
                RelatedBooks = pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Where(b => b.GenreId == existBooks.GenreId)
                .ToList()
            };


            if (userId != null)
            {
                var user = userManager.FindByNameAsync(User.Identity.Name).Result;

                if (user != null)
                {

                    bookDetailVm.HasComment = await pustokDbContex.BookComments.AnyAsync(b => b.AppUserId == user.Id && b.BookId == bookId && b.Status != CommentStatus.Rejected);

                }


            }

            bookDetailVm.TotaComments = await pustokDbContex.BookComments.CountAsync(b => b.BookId == bookId);
            bookDetailVm.RateAvg = bookDetailVm.TotaComments > 0 ? (decimal)await pustokDbContex.BookComments
                .Where(b => b.BookId == bookId)
                .AverageAsync(b => b.Rate) : 0;


            return bookDetailVm;
        }

        public IActionResult DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }
            var exsistComment = pustokDbContex.BookComments
                .Include(c=>c.AppUser)
                .Include(b => b.Book)
                .FirstOrDefault(b => b.Id == id);

            if (exsistComment == null)
            {
                return NotFound();
            }

            pustokDbContex.BookComments.Remove(exsistComment);
            pustokDbContex.SaveChanges();
            return RedirectToAction("Detail", "Book", new { id = exsistComment.BookId });
        }
    }
}