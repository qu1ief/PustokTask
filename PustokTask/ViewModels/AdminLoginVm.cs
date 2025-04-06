using System.ComponentModel.DataAnnotations;

namespace PustokTask.ViewModels;

public class AdminLoginVm
{
    [Required]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; }

    
}
