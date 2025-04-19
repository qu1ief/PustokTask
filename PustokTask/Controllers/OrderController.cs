using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokTask.Data;
using PustokTask.Models;
using PustokTask.ViewModels;

namespace PustokTask.Controllers
{
    
    public class OrderController : Controller
	{
		private readonly PustokDbContex _pustokDbContex;
		private readonly UserManager<AppUser> _userManager;

		public OrderController(PustokDbContex pustokDbContex, UserManager<AppUser> userManager)
		{
			_pustokDbContex = pustokDbContex;
			_userManager = userManager;
		}

		public IActionResult Checkout()
		{
			var user = _userManager.Users.Include(u => u.BasketItems).
				ThenInclude(c=>c.Book)
				.FirstOrDefault(u => u.UserName == User.Identity.Name);



			var checkout = new CheckoutVM
			{
				CheckoutItems = user.BasketItems.Select(b => new CheckoutItemVm
				{
				
					Name = b.Book.Name,
					Price = b.Book.Price,
				
					Count = b.Count
				}).ToList(),

				TotalPrice = user.BasketItems.Sum(b => b.Count * b.Book.Price)
			};
			return View(checkout);
		}


        [HttpPost]
		public IActionResult Checkout(CheckoutVM model)
		{

            var order = model.OrderVM;
            var user = _userManager.Users.Include(u => u.BasketItems).
                 ThenInclude(c => c.Book)
                 .FirstOrDefault(u => u.UserName == User.Identity.Name);



            var checkout = new CheckoutVM
            {
                CheckoutItems = user.BasketItems.Select(b => new CheckoutItemVm
                {

                    Name = b.Book.Name,
                    Price = b.Book.Price,

                    Count = b.Count,
					
					
                }).ToList(),
				OrderVM=order,

                TotalPrice = user.BasketItems.Sum(b => b.Count * b.Book.Price)
            };


            if (!ModelState.IsValid)
			{
				return View(checkout);
			}


			var orders = new Order
			{
				TotalPrice =(int)user.BasketItems.Sum(b => b.Count * b.Book.Price),
				Address = order.Address,
				Town = order.Town
				,
				City = order.City
				,
				ZipCode = order.ZipCode,
				CreatedDate = DateTime.Now,
				Items = user.BasketItems.Select(b => new OrderItem
                {
                    BookId = b.BookId,
                    Count = b.Count
                }).ToList(),

            };

			_pustokDbContex.Orders.Add(orders);

			_pustokDbContex.DbBasketItems.RemoveRange(user.BasketItems);
			_pustokDbContex.SaveChanges();
			return RedirectToAction("Profile" ,"Account"  ,new {tab = "orders"});

        }
    }
}
