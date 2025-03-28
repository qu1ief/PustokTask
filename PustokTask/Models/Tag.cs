using System.ComponentModel.DataAnnotations;

namespace PustokTask.Models
{
    public class Tag : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public List<BookTag> BookTags { get; set; }
    }
}
