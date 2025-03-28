using System.ComponentModel.DataAnnotations.Schema;

namespace PustokTask.Models
{
    public class BookTag : BaseEntity
    {
        [NotMapped]
        public override int Id { get ; set ; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public int TagId { get; set; }
        public Tag Tag { get; set; }    
    }
}
