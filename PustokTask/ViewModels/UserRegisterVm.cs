using System.ComponentModel.DataAnnotations;

namespace PustokTask.ViewModels;

public class UserRegisterVm
{
	[Required]
	public string FullName { get;set; }
	[Required]

	public string UserName { get;set; }
	[Required]
	[EmailAddress]
	public string Email { get;set; }
	[Required]
	[DataType(DataType.Password)]
	public string Password { get;set; }
	[DataType(DataType.Password)]
	[Compare("Password")]
	public string ConfirmPassword { get;set; } 
}
