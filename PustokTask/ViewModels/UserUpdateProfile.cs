using System.ComponentModel.DataAnnotations;

namespace PustokTask.ViewModels;

public class UserUpdateProfile
{
    [Required]
    public string FullName { get; set; }
    [Required]

    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    public string ConfirmPassword { get; set; }
    [DataType(DataType.Password)]

    public string CurentPassword { get; set; }
}
