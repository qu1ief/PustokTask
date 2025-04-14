using System.ComponentModel.DataAnnotations;

namespace PustokTask.ViewModels;

public class ForgotPasswordVm
{
	[Required]
	[EmailAddress]
	[DataType(DataType.EmailAddress)]
	public string Email { get; set; }
}
public class ResertPasswordVm 
{
	
	[Required]
	[DataType(DataType.Password)]
	public string NewPassword { get; set; }
	[DataType(DataType.Password)]
	[Compare("NewPassword")]
	public string ConfirmPassword { get; set; }
	public string Email { get; set; }
	public string Token { get; set; }
}
