namespace PustokTask.ViewModels;

public class CheckoutVM
{
	public List<CheckoutItemVm> CheckoutItems { get; set; }
	public decimal TotalPrice { get; set; }
	public OrderVM OrderVM { get; set; }

}