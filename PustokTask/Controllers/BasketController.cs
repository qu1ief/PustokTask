using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PustokTask.Data;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    public class BasketController : Controller
    {
        private readonly PustokDbContex _pustokDbContex;

        public BasketController(PustokDbContex pustokDbContex)
        {
            _pustokDbContex = pustokDbContex;
        }

        public IActionResult Index()
        {
            return View();
        }



        public IActionResult AddToBasket(int?id)

        {
            if(id == null)
            {
                return NotFound();
            }
            var book= _pustokDbContex.Books
                .FirstOrDefault(b => b.Id == id);

            if(book == null)
            {
                return NotFound();
            }

            List<BasketVM> list = new ();


            var basket = HttpContext.Request.Cookies["basket"];
            if(basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                list = new List<BasketVM>();
            }

            var existItems = list.FirstOrDefault(b => b.BookId == book.Id);

            if (existItems !=null)
            {
                existItems.Count++;
            }
            else
            {
                list.Add(new BasketVM
                {
                    BookId = book.Id,
                    Name = book.Name,
                    MainImage = book.BookImages?.FirstOrDefault(i => i.Status == true)?.Image,
                    BookPrice = book.Price,
                    Count = 1
                });

            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(list));

            return Json(list);
        }
    }
}
