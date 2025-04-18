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
            var existBooks =await pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Include(b => b.BookComments)
                .Include(b => b.BookTags)
                .ThenInclude(b => b.Tag)
                .FirstOrDefaultAsync(b => b.Id == id);
            if (existBooks == null)
                return NotFound();

            BookDetailVm vm = new()
            {
                Book = existBooks,
                RelatedBooks = await pustokDbContex.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .Include(b => b.BookImages)
                .Where(b => b.GenreId == id)
                .ToListAsync()
            };
            if (User.Identity.IsAuthenticated)
            {

                var user =  userManager.FindByNameAsync(User.Identity.Name).Result;
                if (user != null)
                {
                    var commnet = await pustokDbContex.BookComments.FirstOrDefaultAsync(b => b.BookId == id && b.AppUserId == user.Id && b.Status != CommentStatus.Rejected);


                    vm.HasComment = await pustokDbContex.BookComments.AnyAsync(b => b.AppUserId == user.Id && b.BookId == id);
                    vm.TotaComments = await pustokDbContex.BookComments.CountAsync(b => b.BookId == id);
                    vm.RateAvg = vm.TotaComments > 0 ? (decimal)await pustokDbContex.BookComments
                        .Where(b => b.BookId == id)
                        .AverageAsync(b => b.Rate) : 0;


                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                vm.TotaComments = pustokDbContex.BookComments.Count(b => b.BookId == id);
                vm.RateAvg = (decimal)pustokDbContex.BookComments
                    .Where(b => b.BookId == id)
                    .Average(b => b.Rate);
            }
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
        [HttpPost]
        //[Authorize(Roles ="Menber")]
        public IActionResult AddComment(BookComment bookComment)
        {
            

            var user = userManager.FindByNameAsync(User.Identity.Name).Result;
            if (user == null)
            {
                return RedirectToAction("login", "account", new { returnurl = Url.Action("detail", "book", new { id = bookComment.BookId }) });
            }
            bookComment.AppUserId =user.Id;
            pustokDbContex.BookComments.Add(bookComment);
            pustokDbContex.SaveChangesAsync();
            return RedirectToAction(Url.Action("detail", "book", new { id = bookComment.BookId }));
        }
    }
}
