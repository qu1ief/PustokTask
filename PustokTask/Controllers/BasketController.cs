using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly PustokDbContex _pustokDbContex;
        private readonly UserManager<AppUser> _userManager;

        public BasketController(PustokDbContex pustokDbContex, UserManager<AppUser> userManager)
        {
            _pustokDbContex = pustokDbContex;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var basket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> list;
            if (basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                list = new List<BasketVM>();

                foreach (var item in list) 
                {


                    var book = _pustokDbContex.Books
						.Include(b => b.BookImages)
						.FirstOrDefault(b => b.Id == item.BookId);

                    item.MainImage = book.BookImages?.FirstOrDefault()?.Image;
                    item.Name = book.Name;
                    item.BookPrice = book.Price;
				}
            }
            return View(list);
        }



        public async Task<IActionResult> AddToBasket(int? id)

        {
            if (id == null)
            {
                return NotFound();
            }
            var book = _pustokDbContex.Books.Include(x=>x.BookImages)
                .FirstOrDefault(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            List<BasketVM> list = new();


            var basket = HttpContext.Request.Cookies["basket"];
            if (basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                list = new List<BasketVM>();
            }

            var existItems = list.FirstOrDefault(b => b.BookId == book.Id);

            if (existItems != null)
            {
                existItems.Count++;
            }
            else
            {
                list.Add(new BasketVM
                {
                    BookId = book.Id,
                    Name = book.Name,
                    MainImage = book.BookImages?.FirstOrDefault()?.Image,
                    BookPrice = book.Price,
                    Count = 1
                });

            }

            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.Users
                    .Include(b => b.BasketItems)
                    .FirstOrDefault(u => u.UserName == User.Identity.Name);

                var existUserBasketItem = user.BasketItems
                    .FirstOrDefault(b => b.BookId == book.Id);
                if (existUserBasketItem != null)
                {
                    existUserBasketItem.Count++;
                }
                else
                {
                    _pustokDbContex.DbBasketItems.Add(new DbBasketItem
                    {
                        BookId = book.Id,
                        Count = 1,
                        AppUserId = user.Id
                    });

                }
                    _pustokDbContex.SaveChanges();

                var existBook = _pustokDbContex.Books
                    .FirstOrDefault(b => b.Id == book.Id);

            }
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(list));
                return PartialView("_BasketPartial",list);
        }
    }
}