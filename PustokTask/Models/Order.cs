namespace PustokTask.Models;

public class Order
{
	public int Id { get; set; }
	public string ZipCode { get; set; }
	public string City { get; set; }
	public string Town { get; set; }
	public string Address { get; set; }
	public int TotalPrice { get; set; }
	public DateTime CreatedDate { get; set; }
	public string AppUserId { get; set; }
	public AppUser AppUser { get; set; }
	public OrderStatus Status { get; set; } = OrderStatus.Pending;	
	public List<OrderItem> Items { get; set; }
}

public enum OrderStatus
{
	Pending,
	Accepted,
	Cancelled,
	Rejected
}

