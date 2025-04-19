using PustokTask.Models;

namespace PustokTask.ViewModels;

public class UserProfileVm
{
  public UserUpdateProfile UserUpdateProfilee { get; set; }  
    public List<Order> Orders { get; set;} = new List<Order>();
}
