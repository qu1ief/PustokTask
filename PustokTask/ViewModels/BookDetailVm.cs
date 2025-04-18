using PustokTask.Models;

namespace PustokTask.ViewModels
{
    public class BookDetailVm
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; } 
        public bool HasComment { get; set; }
        public int TotaComments {  get; set; }
        public decimal RateAvg { get; set; }
        public BookComment BookComment { get; set; }
    }
}
