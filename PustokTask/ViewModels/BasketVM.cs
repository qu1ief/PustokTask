namespace PustokTask.ViewModels
{
    public class BasketVM
    {
        public int BookId { get; set; }
        public string Name { get; set; }
        public string MainImage { get; set; }
        public decimal BookPrice { get; set; }
        public int Count { get; set; }
        public decimal TotalPrice => Count * BookPrice;
    }
}
