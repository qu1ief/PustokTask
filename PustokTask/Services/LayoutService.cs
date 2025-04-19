

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PPustokTask.Services
{
    public class LayoutService(PustokDbContex pustokDbContext,IHttpContextAccessor httpContextAccessor ,UserManager<AppUser> userManager)
    {
        public List<Setting> GetSettings()
        {
            return pustokDbContext.Settings.ToList();
        }

        public List<Genre> GetGenres()
        {
            return  pustokDbContext.Genres.ToList();
        }

        public List<BasketVM> GetBasketVMs()
        {
            var httpcontext = httpContextAccessor.HttpContext;
            var basket = httpcontext.Request.Cookies["basket"];
            var list = new List<BasketVM>();

            if(basket != null)
            {
                list = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }

            if (httpcontext.User.Identity.IsAuthenticated)
            {
                var user = userManager.Users.Include(x=>x.BasketItems).ThenInclude(x => x.Book).ThenInclude(x => x.BookImages)  
                    .FirstOrDefault(x=>x.UserName == httpcontext.User.Identity.Name);

                list = pustokDbContext.DbBasketItems.Include(x => x.Book).ThenInclude(x => x.BookImages)
                    .Include(x => x.Book).ThenInclude(x => x.BookImages)
                    .Where(x => x.AppUserId == user.Id)
                    .Select(x => new BasketVM
                    {
                        BookId = x.BookId,
                        Count = x.Count,
                        MainImage = x.Book.BookImages.FirstOrDefault(i=>i.Status ==true).Image,
                        Name = x.Book.Name,
                        BookPrice = x.Book.Price
                    }).ToList();
            }

            return list;

        }
    }
}
