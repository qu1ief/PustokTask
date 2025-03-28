using System.ComponentModel.DataAnnotations;

namespace PustokTask.Models
{
    public class BookImage :BaseEntity                                          
    {
        [Required] 
        public string Image { get; set; }
        public bool? Status { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
