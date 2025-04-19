using Microsoft.AspNetCore.Identity;

namespace PustokTask.Models
{
    public class AppUser : IdentityUser
    {
        public string Fulname { get; set; }
        public List<DbBasketItem> BasketItems { get; set; }
    }
}
