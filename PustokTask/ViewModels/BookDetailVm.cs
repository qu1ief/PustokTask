using PustokTask.Models;

namespace PustokTask.ViewModels
{
    public class BookDetailVm
    {
        public Book Book { get; set; }
        public List<Book> RelatedBooks { get; set; } 
    }
}
